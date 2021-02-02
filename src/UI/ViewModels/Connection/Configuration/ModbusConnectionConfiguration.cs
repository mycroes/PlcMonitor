using System.Globalization;
using PlcMonitor.UI.Models;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels.Connection.Configuration
{
    public class ModbusConnectionConfiguration: ValidatableViewModelBase, IConnectionConfiguration
    {
        public string Title { get; } = "Modbus";

        private string? _name;
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string? _host;
        public string? Host
        {
            get => _host;
            set => this.RaiseAndSetIfChanged(ref _host, value);
        }

        private string? _port;
        public string? Port
        {
            get => _port;
            set => this.RaiseAndSetIfChanged(ref _port, value);
        }

        private string? _unitId;
        public string? UnitId
        {
            get => _unitId;
            set => this.RaiseAndSetIfChanged(ref _unitId, value);
        }

        public ModbusConnectionConfiguration()
        {
            this.ValidationRule(x => x.Name, x => !string.IsNullOrWhiteSpace(x), "Name must be set");
            this.ValidationRule(x => x.Host, x => !string.IsNullOrWhiteSpace(x), "Host must be set");
            this.ValidationRule(x => x.Port, x => string.IsNullOrWhiteSpace(x) || ushort.TryParse(x, NumberStyles.Integer, null, out _), "Port must be empty or a valid port number.");
            this.ValidationRule(x => x.UnitId, x => byte.TryParse(x, NumberStyles.Integer, null, out _), "Unit ID must be set");
        }

        public IPlc CreatePlc()
        {
            var port = string.IsNullOrWhiteSpace(Port) ? 502 : ushort.Parse(Port, NumberStyles.Integer);

            return new ModbusPlc(Name!, Host!, port, byte.Parse(UnitId!, NumberStyles.Integer));
        }
    }
}