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

        public S7VariableViewModel(S7Plc plc) : this(plc, default, 1, string.Empty, default)
        { }

        public S7VariableViewModel(S7Plc plc, TypeCode typeCode, int length, string address, VariableStateViewModel? state)
            : base(plc, typeCode, length, state)
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
