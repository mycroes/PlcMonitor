using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class S7Variable : Variable
    {
        public string Address { get; }

        public S7Variable(string? name, string address, TypeCode typeCode, int length, VariableState? state)
            : base(name, typeCode, length, state)
        {
            Address = address;
        }
    }
}