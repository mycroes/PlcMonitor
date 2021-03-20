using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlcMonitor.UI.Models.PlcData;
using PlcMonitor.UI.Models.Plcs;
using PlcMonitor.UI.Models.Plcs.S7;
using PlcMonitor.UI.Models.Plcs.Modbus;
using PlcMonitor.UI.ViewModels;
using Sally7;
using System.Runtime.InteropServices;
using NModbus;

namespace PlcMonitor.UI.Services
{
    public class PlcInteractionManager : IPlcInteractionManager
    {
        public VariableViewModel CreateVariable(IPlc plc)
        {
            return plc switch
            {
                S7Plc s7plc => new S7VariableViewModel(s7plc),
                ModbusPlc modbusPlc => new ModbusVariableViewModel(modbusPlc),
                _ => throw new NotImplementedException()
            };
        }

        public Task Read(IPlc plc, IEnumerable<VariableViewModel> variables)
        {
            switch (plc)
            {
                case ModbusPlc modbusPlc:
                    return ReadModbus(modbusPlc, variables.Cast<ModbusVariableViewModel>());
                case S7Plc s7plc:
                    return ReadS7(s7plc, variables.Cast<S7VariableViewModel>());
            }

            foreach (var variable in variables)
            {
                variable.PushValue(new ReceivedValue(new Random().Next(3), DateTimeOffset.Now));
            }

            return Task.CompletedTask;
        }

        public Task Write(IPlc plc, IDictionary<VariableViewModel, object?> variableValues)
        {
            switch (plc)
            {
                case ModbusPlc modbusPlc:
                    return WriteModbus(modbusPlc, variableValues.Select(x => (Variable: (ModbusVariableViewModel)x.Key, Value: x.Value)));
                case S7Plc s7plc:
                    return WriteS7(s7plc, variableValues.Select(x => (Variable: (S7VariableViewModel)x.Key, Value: x.Value)));
            }

            return Task.CompletedTask;
        }

        private IEnumerable<Bundle<T>> BundleVariables<T>(IEnumerable<T> input, int maxGap, Func<T, int> startSelector, Func<T, int> lengthSelector)
        {
            using (var enumerator = input.OrderBy(startSelector).GetEnumerator())
            {
                if (!enumerator.MoveNext()) yield break;

                var variable = enumerator.Current!;
                var start = startSelector(variable);
                var end = start + lengthSelector(variable);
                List<T> variables = new() { variable };

                while (enumerator.MoveNext())
                {
                    variable = enumerator.Current!;
                    var nextStart = startSelector(variable);
                    var nextEnd = nextStart + lengthSelector(variable);
                    if (nextStart < end + maxGap)
                    {
                        if (nextEnd > end) end = nextEnd;
                        variables.Add(variable);
                    }
                    else
                    {
                        yield return new Bundle<T>(start, end, variables);
                        start = nextStart;
                        end = nextEnd;
                        variables = new() { variable };
                    }
                }

                if (variables.Any())
                {
                    yield return new Bundle<T>(start, end, variables);
                }
            }
        }

        private class Bundle<T>
        {
            public int Start { get; }
            public int Length { get; }
            public IEnumerable<T> Elements { get; }

            public Bundle(in int start, in int length, in IEnumerable<T> elements)
            {
                Start = start;
                Length = length;
                Elements = elements;
            }
        }

        private async Task ReadModbus(ModbusPlc plc, IEnumerable<ModbusVariableViewModel> variables)
        {
            const int maxGap = 32;
            var groups = variables.GroupBy(v => v.ObjectType);
            foreach (var group in groups)
            {
                // Hack, logic only works for holding / input registers
                Func<IModbusMaster, Bundle<ModbusVariableViewModel>, Task<ushort[]>> read = group.Key switch
                {
                    ObjectType.HoldingRegister => (m, bundle) => m.ReadHoldingRegistersAsync(plc.UnitId, (ushort)bundle.Start, (ushort)bundle.Length),
                    ObjectType.InputRegister => (m, bundle) => m.ReadInputRegistersAsync(plc.UnitId, (ushort)bundle.Start, (ushort)bundle.Length),
                    _ => throw new ArgumentOutOfRangeException($"No support for reading ObjectType {group.Key}.")
                };

                foreach (var bundle in BundleVariables(group, maxGap, v => v.Address, v => GetNativeLength(v, sizeof(ushort))))
                {
                    var res = await plc.Schedule(async conn => await read(((ModbusPlcConnection)conn).ModbusMaster, bundle))!;
                    ParseReadValues(bundle, res);
                }
            }
        }

        private static void ParseReadValues(Bundle<ModbusVariableViewModel> bundle, ushort[] data)
        {
            var order = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }.AsSpan();
            var input = data.AsSpan();

            foreach (var v in bundle.Elements)
            {
                var bytes = MemoryMarshal.Cast<ushort, byte>(input.Slice(v.Address - bundle.Start));
                object value = v.Length > 1 ? v.TypeCode switch
                {
                    TypeCode.Byte => DataConverter.ConvertToRuntime<byte[]>(bytes, order),
                    TypeCode.Double => DataConverter.ConvertToRuntime<double[]>(bytes, order),
                    TypeCode.Int16 => DataConverter.ConvertToRuntime<short[]>(bytes, order),
                    TypeCode.Int32 => DataConverter.ConvertToRuntime<int[]>(bytes, order),
                    TypeCode.Int64 => DataConverter.ConvertToRuntime<long[]>(bytes, order),
                    TypeCode.SByte => DataConverter.ConvertToRuntime<sbyte[]>(bytes, order),
                    TypeCode.Single => DataConverter.ConvertToRuntime<float[]>(bytes, order),
                    TypeCode.UInt16 => DataConverter.ConvertToRuntime<ushort[]>(bytes, order),
                    TypeCode.UInt32 => DataConverter.ConvertToRuntime<uint[]>(bytes, order),
                    TypeCode.UInt64 => DataConverter.ConvertToRuntime<ulong[]>(bytes, order),
                    _ => throw new ArgumentException($"Unsupported type {v.TypeCode}")
                } : v.TypeCode switch
                {
                    // First case has explicit object cast to avoid implicit cast to double
                    TypeCode.Byte => (object) DataConverter.ConvertToRuntime<byte>(bytes, order),
                    TypeCode.Double => DataConverter.ConvertToRuntime<double>(bytes, order),
                    TypeCode.Int16 => DataConverter.ConvertToRuntime<short>(bytes, order),
                    TypeCode.Int32 => DataConverter.ConvertToRuntime<int>(bytes, order),
                    TypeCode.Int64 => DataConverter.ConvertToRuntime<long>(bytes, order),
                    TypeCode.SByte => DataConverter.ConvertToRuntime<sbyte>(bytes, order),
                    TypeCode.Single => DataConverter.ConvertToRuntime<float>(bytes, order),
                    TypeCode.UInt16 => DataConverter.ConvertToRuntime<ushort>(bytes, order),
                    TypeCode.UInt32 => DataConverter.ConvertToRuntime<uint>(bytes, order),
                    TypeCode.UInt64 => DataConverter.ConvertToRuntime<ulong>(bytes, order),
                    _ => throw new ArgumentException($"Unsupported type {v.TypeCode}")
                };

                v.PushValue(new ReceivedValue(value, DateTimeOffset.Now));
            }
        }

        private async Task WriteModbus(ModbusPlc plc, IEnumerable<(ModbusVariableViewModel variable, object value)> variableValues)
        {
            var groups = variableValues.GroupBy(v => v.variable.ObjectType);
            foreach (var group in groups)
            {
                // Hack, logic only works for holding registers
                Func<IModbusMaster, ushort, ushort[], Task> write = group.Key switch
                {
                    ObjectType.HoldingRegister => (m, start, data) => m.WriteMultipleRegistersAsync(plc.UnitId, start, data),
                    _ => throw new ArgumentOutOfRangeException($"No support for writing ObjectType {group.Key}.")
                };

                foreach (var bundle in BundleVariables(group, 0, vv => vv.variable.Address, vv => GetNativeLength(vv.variable, sizeof(ushort))))
                {
                    var data = new ushort[bundle.Length];
                    foreach (var (variable, value) in bundle.Elements)
                    {
                        WriteValue(data, variable.Address - bundle.Start, variable.TypeCode, variable.Length, value);
                    }

                    await plc.Schedule(async conn => await write(((ModbusPlcConnection)conn).ModbusMaster, (ushort)bundle.Start, data));
                }
            }
        }

        private void WriteValue(ushort[] data, int offset, TypeCode typeCode, int length, object value)
        {
            // Hack: move span and bytes outside of ReadValue
            var span = data.AsSpan().Slice(offset);
            var bytes = MemoryMarshal.Cast<ushort, byte>(span);

            // Hack: no array support
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    bytes[0] = ((bool)value) ? 1 : 0;
                    break;
                case TypeCode.Byte:
                    bytes[0] = (byte)value;
                    break;
                case TypeCode.Int16:
                    bytes[0] = (byte)(((short)value) >> 8);
                    bytes[1] = (byte)(short)value;
                    break;
                case TypeCode.Int32:
                    bytes[0] = (byte)(((int)value) >> 24);
                    bytes[1] = (byte)(((int)value) >> 16);
                    bytes[2] = (byte)(((int)value) >> 8);
                    bytes[3] = (byte)(int)value;
                    break;
                case TypeCode.UInt16:
                    bytes[0] = (byte)(((ushort)value) >> 8);
                    bytes[1] = (byte)(ushort)value;
                    break;
                case TypeCode.UInt32:
                    bytes[0] = (byte)(((uint)value) >> 24);
                    bytes[1] = (byte)(((uint)value) >> 16);
                    bytes[2] = (byte)(((uint)value) >> 8);
                    bytes[3] = (byte)(uint)value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported conversion for TypeCode {typeCode}.");
            }
        }

        private int GetNativeLength(VariableViewModel variable, int nativeSize)
        {
            return (GetByteLength(variable) + (nativeSize - 1)) / nativeSize;
        }

        private int GetByteLength(VariableViewModel variable)
        {
            int Len(int elementSize) => variable.Length * elementSize;

            return variable.TypeCode switch
            {
                TypeCode.Boolean => variable.Length + 7 / 8,
                TypeCode.Byte => variable.Length,
                TypeCode.Double => Len(sizeof(double)),
                TypeCode.Int16 => Len(sizeof(short)),
                TypeCode.Int32 => Len(sizeof(int)),
                TypeCode.Int64 => Len(sizeof(long)),
                TypeCode.SByte => variable.Length,
                TypeCode.Single => Len(sizeof(float)),
                TypeCode.String => variable.Length,
                TypeCode.UInt16 => Len(sizeof(ushort)),
                TypeCode.UInt32 => Len(sizeof(uint)),
                TypeCode.UInt64 => Len(sizeof(ulong)),
                _ => throw new ArgumentOutOfRangeException(nameof(variable))
            };
        }

        private async Task ReadS7(S7Plc plc, IEnumerable<S7VariableViewModel> variables)
        {
            object GetValue(IDataItem dataItem)
            {
                return dataItem.GetType().GetProperty(nameof(IDataItem<int>.Value))!.GetValue(dataItem)!;
            }

            var pairs = variables.Select(v => (Variable: v, DataItem: DataItemBuilder.BuildDataItem(v.Address, v.Length))).ToList();

            await plc.Schedule(conn => ((IS7PlcConnection)conn).Read(pairs.Select(p => p.DataItem)));

            foreach (var (variable, dataItem) in pairs)
            {
                variable.PushValue(new ReceivedValue(GetValue(dataItem), DateTimeOffset.Now));
            }
        }

        private Task WriteS7(S7Plc plc, IEnumerable<(S7VariableViewModel variable, object value)> variableValues)
        {
            void SetValue(IDataItem dataItem, object? value)
            {
                dataItem.GetType().GetProperty(nameof(IDataItem<int>.Value))!.SetValue(dataItem, value);
            }

            IDataItem BuildAndApply((S7VariableViewModel variable, object? value) variableValue)
            {
                var (variable, value) = variableValue;
                var di = DataItemBuilder.BuildDataItem(variable.Address, variable.Length);
                SetValue(di, value);

                return di;
            }

            return plc.Schedule(conn => ((IS7PlcConnection)conn).Write(variableValues.Select(BuildAndApply)));
        }
    }
}