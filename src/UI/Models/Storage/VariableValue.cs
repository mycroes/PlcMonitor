using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class VariableState
    {
        public ValueWithTypeCode Value { get; }
        public DateTimeOffset LastChange { get; }
        public DateTimeOffset LastRead { get; }

        public VariableState(ValueWithTypeCode value, DateTimeOffset lastChange, DateTimeOffset lastRead)
        {
            Value = value;
            LastChange = lastChange;
            LastRead = lastRead;
        }
    }
}