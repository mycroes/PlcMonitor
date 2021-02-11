using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class VariableState
    {
        public object Value { get; }
        public DateTimeOffset LastChange { get; }
        public DateTimeOffset LastRead { get; }

        public VariableState(object value, DateTimeOffset lastChange, DateTimeOffset lastRead)
        {
            Value = value;
            LastChange = lastChange;
            LastRead = lastRead;
        }
    }
}