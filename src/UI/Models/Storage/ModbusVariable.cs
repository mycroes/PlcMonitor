using System;
using PlcMonitor.UI.Models.Modbus;

namespace PlcMonitor.UI.Models.Storage
{
    public class ModbusVariable : Variable
    {
        public ObjectType ObjectType { get; }

        public ushort Address { get; }

        public ModbusVariable(string? name, ObjectType objectType, ushort address, TypeCode typeCode, int length, VariableState? state)
            : base(name, typeCode, length, state)
        {
            ObjectType = objectType;
            Address = address;
        }
    }
}