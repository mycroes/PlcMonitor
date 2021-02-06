using System;

namespace PlcMonitor.UI.Models.Storage
{
    public class ValueWithTypeCode
    {
        public TypeCode TypeCode { get; }
        public bool IsArray { get; }
        public object Value { get; }

        public ValueWithTypeCode(TypeCode typeCode, bool isArray, object value)
        {
            TypeCode = typeCode;
            IsArray = isArray;
            Value = value;
        }
    }
}