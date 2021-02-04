using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class VariableValue
    {
        public byte[] RawValue { get; }
        public DateTimeOffset LastChange { get; }
        public DateTimeOffset LastRead { get; }

        public VariableValue(byte[] rawValue, DateTimeOffset lastChange, DateTimeOffset lastRead)
        {
            RawValue = rawValue;
            LastChange = lastChange;
            LastRead = lastRead;
        }
    }
}