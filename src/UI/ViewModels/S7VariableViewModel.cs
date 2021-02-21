using System;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.S7;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels
{
    public class S7VariableViewModel : VariableViewModel
    {
        private string _address;
        public string Address
        {
            get => _address;
            set => this.RaiseAndSetIfChanged(ref _address, value);
        }

        public S7VariableViewModel(S7Plc plc) : this(plc, default, string.Empty, default, 1, default)
        { }

        public S7VariableViewModel(S7Plc plc, string? name, string address, TypeCode typeCode, int length, VariableStateViewModel? state)
            : base(plc, name, typeCode, length, state)
        {
            _address = address;

            this.ValidationRule(x => x.Address, plc.IsValidAddress, "Invalid address");
        }

        protected override void OnUpdate()
        {
            if (AddressParser.TryParse(Address, out _, out var typeCode, out _, out _)) TypeCode = typeCode;
        }
    }
}
