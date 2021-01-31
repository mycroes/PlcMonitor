using System.Reactive;
using PlcMonitor.UI.Models;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels.Explorer
{
    public class PlcConnectionNode : IExplorerNode
    {
        private readonly ProjectViewModel _project;
        private readonly Plc _plc;
        public string Name => _plc.Name;

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public PlcConnectionNode(ProjectViewModel project, Plc plc)
        {
            _project = project;
            _plc = plc;

            DeleteCommand = ReactiveCommand.Create(() => {project.Plcs.Remove(plc); });
        }
    }
}
