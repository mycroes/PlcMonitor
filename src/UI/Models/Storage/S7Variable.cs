using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class S7Variable : Variable
    {
        public string Address { get; }

        public S7Variable(string address, TypeCode typeCode, int length, VariableState? state)
            : base(typeCode, length, state)
        {
            Address = address;
        }
    }
}