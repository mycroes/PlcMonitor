using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PlcMonitor.UI.DI;
using PlcMonitor.UI.ViewModels.Connection.Configuration;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels.Explorer
{
    public class AddConnectionNode : ReactiveObject, IExplorerNode
    {
        private readonly ProjectViewModel _project;
        private readonly PlcViewModelFactory _plcViewModelFactory;

        public string Name { get; } = "Add connection";

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, bool> TestCommand { get; }

        private ConnectionConfigurationBase? _configuration;

        public ConnectionConfigurationBase? Configuration
        {
            get => _configuration;
            set => this.RaiseAndSetIfChanged(ref _configuration, value);
        }

        private IReadOnlyList<ConnectionConfigurationBase> _configurations;

        public IReadOnlyList<ConnectionConfigurationBase> Configurations
        {
            get => _configurations;
            set => this.RaiseAndSetIfChanged(ref _configurations, value);
        }

        public IObservable<bool?> TestResult { get; }

        public AddConnectionNode(ProjectViewModel project, PlcViewModelFactory plcViewModelFactory)
        {
            _project = project;
            _plcViewModelFactory = plcViewModelFactory;

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
            _project.Plcs.Add(_plcViewModelFactory.Invoke(
                Configuration!.CreatePlc(), Configuration!.Name!, Enumerable.Empty<VariableViewModel>()));

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

        private IEnumerable<ConnectionConfigurationBase> BuildConfigurations()
        {
            yield return new ModbusConnectionConfiguration();
            yield return new S7ConnectionConfiguration();
        }
    }
}
