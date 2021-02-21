using System;
using System.Collections;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.PlcData;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels
{
    public abstract class VariableViewModel : ValidatableViewModelBase
    {
        protected IPlc Plc { get; }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
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

        protected VariableViewModel(IPlc plc) : this(plc, default, default, 1, default)
        {
        }

        protected VariableViewModel(IPlc plc, string? name, TypeCode typeCode, int length, VariableStateViewModel? state)
        {
            Plc = plc;
            _name = name;
            _typeCode = typeCode;
            _length = length;
            _state = state;

            this.ValidationRule(x => x.Length, x => x > 0, "Length must be positive and at least 1");
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

        public void Update()
        {
            OnUpdate();
            State = null;
        }

        protected abstract void OnUpdate();
    }
}
