using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class VariableValue
    {
        public ValueWithTypeCode Value { get; }
        public DateTimeOffset LastChange { get; }
        public DateTimeOffset LastRead { get; }

        public VariableValue(ValueWithTypeCode value, DateTimeOffset lastChange, DateTimeOffset lastRead)
        {
            Value = value;
            LastChange = lastChange;
            LastRead = lastRead;
        }
    }
}