using System;
using System.Collections;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.PlcData;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels
{
    public class VariableViewModel : ValidatableViewModelBase
    {
        private readonly IPlc _plc;

        private string _address;
        public string Address
        {
            get => _address;
            set => this.RaiseAndSetIfChanged(ref _address, value);
        }

        private TypeCode _typeCode;
        public TypeCode TypeCode
        {
            get => _typeCode;
            set => this.RaiseAndSetIfChanged(ref _typeCode, value);
        }

        private int _length;
        public int Length
        {
            get => _length;
            set => this.RaiseAndSetIfChanged(ref _length, value);
        }

        private VariableStateViewModel? _state;
        public VariableStateViewModel? State
        {
            get => _state;
            set => this.RaiseAndSetIfChanged(ref _state, value);
        }

        public VariableViewModel(IPlc plc) : this(plc, string.Empty, default, 1, default)
        {
        }

        public VariableViewModel(IPlc plc, string address, TypeCode typeCode, int length, VariableStateViewModel? state)
        {
            _plc = plc;
            _address = address;
            _typeCode = typeCode;
            _length = length;
            _state = state;

            this.ValidationRule(x => x.Address, plc.IsValidAddress, "Invalid address");
        }

        public void PushValue(ReceivedValue next)
        {
            if (State != null && StructuralComparisons.StructuralEqualityComparer.Equals(next.Value, State.Value))
            {
                State.LastRead.OnNext(next.Timestamp);
            }
            else
            {
                State = new VariableStateViewModel(next.Value, next.Timestamp, next.Timestamp);
            }
        }
    }
}
