using System;
using System.Text.Json.Serialization;

namespace PlcMonitor.UI.Models.Storage
{
    public abstract class Variable
    {
        public string? Name { get; }

        public TypeCode TypeCode { get; }

        public int Length { get; }

        [JsonIgnore]
        public VariableState? State { get; set; }

        protected Variable(string? name, TypeCode typeCode, int length, VariableState? state)
        {
            Name = name;
            TypeCode = typeCode;
            Length = length;
            State = state;
        }
    }
}