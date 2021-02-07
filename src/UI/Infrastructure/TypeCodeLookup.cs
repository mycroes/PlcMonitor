using System;

namespace PlcMonitor.UI.Infrastructure
{
    public static class TypeCodeLookup
    {
        public static Type TypeCodeToType(in TypeCode typeCode)
        {
            return typeCode switch
            {
                TypeCode.Boolean => typeof(bool),
                TypeCode.Byte => typeof(byte),
                TypeCode.Char => typeof(char),
                TypeCode.DateTime => typeof(DateTime),
                TypeCode.Double => typeof(double),
                TypeCode.Int16 => typeof(short),
                TypeCode.Int32 => typeof(int),
                TypeCode.Int64 => typeof(long),
                TypeCode.SByte => typeof(sbyte),
                TypeCode.Single => typeof(float),
                TypeCode.String => typeof(string),
                TypeCode.UInt16 => typeof(ushort),
                TypeCode.UInt32 => typeof(uint),
                TypeCode.UInt64 => typeof(ulong),
                _ => throw new ArgumentException($"No known type found for {nameof(TypeCode)} {typeCode}.", nameof(typeCode))
            };
        }
    }
}