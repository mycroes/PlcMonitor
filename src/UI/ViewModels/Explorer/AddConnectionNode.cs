using System;
using System.Globalization;
using System.Reactive;
using PlcMonitor.UI.Models;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.ViewModels.Explorer
{
    public class AddConnectionNode : ReactiveValidationObject, IExplorerNode
    {
        private readonly ProjectViewModel _project;

        public string Name { get; } = "Add connection";

        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        private string? _plcName;
        public string? PlcName
        {
            get => _plcName;
            set => this.RaiseAndSetIfChanged(ref _plcName, value);
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

        public AddConnectionNode(ProjectViewModel project)
        {
            _project = project;

            AddCommand = ReactiveCommand.Create(Add, this.IsValid());

            this.ValidationRule(x => x.PlcName, x => !string.IsNullOrWhiteSpace(x), "Name must be set");
            this.ValidationRule(x => x.Host, x => !string.IsNullOrWhiteSpace(x), "Host must be set");
            this.ValidationRule(x => x.LocalTsap, IsValidTsap, "Local TSAP must be in the format 3A:01");
            this.ValidationRule(x => x.RemoteTsap, IsValidTsap, "Remote TSAP must be in the format 3A:01");
        }

        private void Add()
        {
            _project.Plcs.Add(new S7Plc(PlcName!, Host!, ParseTsap(LocalTsap!), ParseTsap(RemoteTsap!)));

            PlcName = null;
            Host = null;
            LocalTsap = null;
            RemoteTsap = null;
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
