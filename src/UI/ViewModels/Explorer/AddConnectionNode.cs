using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PlcMonitor.UI.ViewModels.Connection.Configuration;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels.Explorer
{
    public class AddConnectionNode : ReactiveObject, IExplorerNode
    {
        private readonly ProjectViewModel _project;

        public string Name { get; } = "Add connection";

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, bool> TestCommand { get; }

        private IConnectionConfiguration? _configuration;

        public IConnectionConfiguration? Configuration
        {
            get => _configuration;
            set => this.RaiseAndSetIfChanged(ref _configuration, value);
        }

        private IReadOnlyList<IConnectionConfiguration> _configurations;

        public IReadOnlyList<IConnectionConfiguration> Configurations
        {
            get => _configurations;
            set => this.RaiseAndSetIfChanged(ref _configurations, value);
        }

        public IObservable<bool?> TestResult { get; }

        public AddConnectionNode(ProjectViewModel project)
        {
            _project = project;

            _configurations = BuildConfigurations().ToList();
            _configuration = _configurations.First();

            var isValid = this.WhenAnyValue(x => x.Configuration).Where(c => c is { }).SelectMany(c => c!.IsValid());
            AddCommand = ReactiveCommand.Create(Add, isValid);
            TestCommand = ReactiveCommand.CreateFromTask(TestConnection, isValid);

            TestResult = Observable.Merge<bool?>(
                this
                    .WhenAnyValue(x => x.Configuration)
                    .Where(c => c is { })
                    .SelectMany(c => Observable
                        .FromEventPattern<PropertyChangedEventArgs>(c!, nameof(c.PropertyChanged))
                        .Select(_ => default(bool?))),
                this.WhenAnyValue(x => x.Configuration).Select(_ => default(bool?)),
                TestCommand.AsObservable().Select(x => (bool?) x));
        }

        private void Add()
        {
            _project.Plcs.Add(Configuration!.CreatePlc());

            Configurations = BuildConfigurations().ToList();
            Configuration = Configurations.First();
        }

        public async Task<bool> TestConnection()
        {
            try {
                var plc = Configuration!.CreatePlc();
                var conn = plc.CreateConnection();
                await conn.Open();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private IEnumerable<IConnectionConfiguration> BuildConfigurations()
        {
            yield return new ModbusConnectionConfiguration();
            yield return new S7ConnectionConfiguration();
        }
    }
}
