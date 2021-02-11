using System;
using System.Text.Json.Serialization;

namespace PlcMonitor.UI.Models.Storage
{
    public class Variable
    {
        public TypeCode TypeCode { get; }

        public int Length { get; }

        [JsonIgnore]
        public VariableState? State { get; set; }

        public Variable(TypeCode typeCode, int length, VariableState? state)
        {
            TypeCode = typeCode;
            Length = length;
            State = state;
        }
    }
}