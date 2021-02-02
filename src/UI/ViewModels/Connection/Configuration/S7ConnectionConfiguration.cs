using System;
using System.Globalization;
using PlcMonitor.UI.Models;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.ViewModels.Connection.Configuration
{
    public class S7ConnectionConfiguration : ValidatableViewModelBase, IConnectionConfiguration
    {
        public string Title { get; } = "S7";

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

        private string? _localTsap;
        public string? LocalTsap
        {
            get => _localTsap;
            set => this.RaiseAndSetIfChanged(ref _localTsap, value);
        }

        private string? _remoteTsap;
        public string? RemoteTsap
        {
            get => _remoteTsap;
            set => this.RaiseAndSetIfChanged(ref _remoteTsap, value);
        }

        public S7ConnectionConfiguration()
        {
            this.ValidationRule(x => x.Name, x => !string.IsNullOrWhiteSpace(x), "Name must be set");
            this.ValidationRule(x => x.Host, x => !string.IsNullOrWhiteSpace(x), "Host must be set");
            this.ValidationRule(x => x.LocalTsap, IsValidTsap, "Local TSAP must be in the format 3A:01");
            this.ValidationRule(x => x.RemoteTsap, IsValidTsap, "Remote TSAP must be in the format 3A:01");
        }

        public IPlc CreatePlc()
        {
            return new S7Plc(Name!, Host!, ParseTsap(LocalTsap!), ParseTsap(RemoteTsap!));
        }

        private bool IsValidTsap(string? input)
        {
            if (input == null) return false;

            if (input.Length == 4)
            {
                return int.TryParse(input, System.Globalization.NumberStyles.HexNumber, null, out _);
            }

            if (input.Length == 5)
            {
                return byte.TryParse(input.AsSpan().Slice(0, 2), NumberStyles.HexNumber, null, out _)
                    && byte.TryParse(input.AsSpan().Slice(3, 2), NumberStyles.HexNumber, null, out _);
            }

            return false;
        }

        private Tsap ParseTsap(string input)
        {
            return new Tsap(
                byte.Parse(input.AsSpan().Slice(0, 2), NumberStyles.HexNumber),
                byte.Parse(input.AsSpan().Slice(input.Length - 2), NumberStyles.HexNumber));
        }
    }
}