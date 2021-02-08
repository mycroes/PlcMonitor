using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class Variable
    {
        public string Address { get; }

        public TypeCode TypeCode { get; }

        public int Length { get; }

        public VariableState? State { get; }

        public Variable(string address, TypeCode typeCode, int length, VariableState? state)
        {
            Address = address;
            TypeCode = typeCode;
            Length = length;
            State = state;
        }
    }
}