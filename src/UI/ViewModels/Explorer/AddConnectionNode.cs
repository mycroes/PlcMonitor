using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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

        private IConnectionConfiguration _configuration;

        public IConnectionConfiguration Configuration
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

        public AddConnectionNode(ProjectViewModel project)
        {
            _project = project;

            _configurations = BuildConfigurations().ToList();
            _configuration = _configurations.First();

            AddCommand = ReactiveCommand.Create(Add, this.WhenAnyValue(x => x.Configuration).SelectMany(c => c.IsValid()));
        }

        private void Add()
        {
            _project.Plcs.Add(Configuration.CreatePlc());

            Configurations = BuildConfigurations().ToList();
            Configuration = Configurations.First();
        }

        private IEnumerable<IConnectionConfiguration> BuildConfigurations()
        {
            yield return new S7ConnectionConfiguration();
        }
    }
}
