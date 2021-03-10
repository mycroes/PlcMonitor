using System;
using System.Text;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels
{
    public class WriteVariableValueViewModel : ValidatableViewModelBase
    {
        private delegate bool Parser<T>(string input, out T output);

        private readonly PlcViewModel _plc;

        public VariableViewModel Variable { get; }

        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private object? _parsedValue;
        public object? ParsedValue => _parsedValue;

        public WriteVariableValueViewModel(PlcViewModel plc, VariableViewModel variable)
        {
            _plc = plc;
            Variable = variable;

            this.ValidationRule(x => x.Value, v => TryParseValue(v, out _parsedValue), "Invalid value");
        }

        private bool TryParseValue(string input, out object? value)
        {
            bool ValidateString(out object? value) {
                value = input;
                var bytes = Encoding.ASCII.GetBytes(input);

                return bytes.Length <= Variable.Length;
            }

            value = null;

            return Variable.TypeCode switch
            {
                TypeCode.Boolean => TryParse<bool>(input, bool.TryParse, out value),
                TypeCode.Byte => TryParse<byte>(input, byte.TryParse, out value),
                TypeCode.Double => TryParse<double>(input, double.TryParse, out value),
                TypeCode.Int16 => TryParse<short>(input, short.TryParse, out value),
                TypeCode.Int32 => TryParse<int>(input, int.TryParse, out value),
                TypeCode.Int64 => TryParse<long>(input, long.TryParse, out value),
                TypeCode.SByte => TryParse<sbyte>(input, sbyte.TryParse, out value),
                TypeCode.Single => TryParse<float>(input, float.TryParse, out value),
                TypeCode.String => ValidateString(out value),
                TypeCode.UInt16 => TryParse<ushort>(input, ushort.TryParse, out value),
                TypeCode.UInt32 => TryParse<uint>(input, uint.TryParse, out value),
                TypeCode.UInt64 => TryParse<ulong>(input, ulong.TryParse, out value),
                _ => false
            };
        }

        private bool TryParse<T>(string input, Parser<T> parser, out object? value)
        {
            if (Variable.Length > 1)
            {
                var inputs = input.Split(',', StringSplitOptions.TrimEntries);
                if (inputs.Length != Variable.Length) {
                    value = null;
                    return false;
                }

                var res = new T[inputs.Length];
                for (var i = 0; i < inputs.Length; i++)
                {
                    if (!parser(inputs[i], out res[i]))
                    {
                        value = null;
                        return false;
                    }
                }

                value = res;
                return true;
            }
            else
            {
                var res = parser(input, out var parsed);
                value = parsed;

                return res;
            }
        }
    }
}