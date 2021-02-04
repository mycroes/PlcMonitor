using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.PlcData;
using PlcMonitor.UI.Models.Storage;
using PlcMonitor.UI.ViewModels;
using PlcMonitor.UI.ViewModels.Explorer;

namespace PlcMonitor.UI.Services
{
    public class MapperService : IMapperService
    {
        public Project MapToStorage(IEnumerable<PlcConnectionNode> plcs)
        {
            return new Project(plcs.Select(MapToStorage));
        }

        private static PlcType GetPlcType(IPlc plc)
        {
            return plc switch
            {
                ModbusPlc _ => PlcType.Modbus,
                S7Plc _ => PlcType.S7,
                _ => throw new ArgumentException($"Unsupported PLC {plc}.", nameof(plc))
            };
        }

        private static Plc MapToStorage(PlcConnectionNode plc)
        {
            return new Plc(plc.Name, GetPlcType(plc.Plc), plc.Variables.Select(MapToStorage));
        }

        private static Variable MapToStorage(VariableViewModel variable)
        {
            return new Variable(variable.Address, variable.Type, variable.Length, MapToStorage(variable.Value.Value));
        }

        private static VariableValue? MapToStorage(ReceivedValue? value)
        {
            if (value == null) return null;

            // TODO implement last change
            return new VariableValue(SerializeValue(value.Value), value.Timestamp, value.Timestamp);
        }

        private static byte[] SerializeValue(object value)
        {
            return value switch
            {
                long v => BitConverter.GetBytes(v),
                ulong v => BitConverter.GetBytes(v),
                double v => BitConverter.GetBytes(v),
                int v => BitConverter.GetBytes(v),
                uint v => BitConverter.GetBytes(v),
                float v => BitConverter.GetBytes(v),
                short v => BitConverter.GetBytes(v),
                ushort v => BitConverter.GetBytes(v),
                byte v => new[] { v },
                sbyte v => new[] { unchecked((byte)v) },
                bool v => new byte[] { v ? 1 : 0 },
                string v => Encoding.UTF8.GetBytes(v),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}