using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class S7Variable : Variable
    {
        public string Address { get; }

        public S7Variable(TypeCode typeCode, int length, string address, VariableState? state)
            : base(typeCode, length, state)
        {
            Address = address;
        }
    }
}