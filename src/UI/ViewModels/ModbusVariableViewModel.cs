using System;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.Modbus;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels
{
    public class ModbusVariableViewModel : VariableViewModel
    {
        private ObjectType _objectType;
        public ObjectType ObjectType
        {
            get => _objectType;
            set => this.RaiseAndSetIfChanged(ref _objectType, value);
        }

        private ushort _address;
        public ushort Address
        {
            get => _address;
            set => this.RaiseAndSetIfChanged(ref _address, value);
        }

        public ModbusVariableViewModel(ModbusPlc plc) : this(plc, default, default, default, 1, default)
        {
        }

        public ModbusVariableViewModel(ModbusPlc plc, ObjectType objectType, ushort address, TypeCode typeCode, int length, VariableStateViewModel? state)
            : base(plc, typeCode, length, state)
        {
            _objectType = objectType;
            _address = address;

            this.ValidationRule(x => x.Address, v => v >= 0, "Address must be a non-negative number.");
        }

        protected override void OnUpdate()
        {
            switch (ObjectType)
            {
                case ObjectType.Coil:
                case ObjectType.DiscreteInput:
                    TypeCode = TypeCode.Boolean;
                    break;
            }
        }
    }
}