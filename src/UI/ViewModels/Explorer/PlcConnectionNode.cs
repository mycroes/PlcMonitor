using System.Reactive;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels.Explorer
{
    public class PlcConnectionNode : IExplorerNode
    {
        private readonly ProjectViewModel _project;

        public PlcViewModel Plc { get; }

        public string Name => Plc.Name;

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public PlcConnectionNode(ProjectViewModel project, PlcViewModel plc)
        {
            _project = project;
            Plc = plc;

            DeleteCommand = ReactiveCommand.Create(() => { project.Plcs.Remove(plc); });
        }
    }
}
