using System.Reactive;
using PlcMonitor.UI.Models;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels.Explorer
{
    public class AddConnectionNode : ViewModelBase, IExplorerNode
    {
        public string Name { get; } = "Add connection";

        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        public AddConnectionNode(ProjectViewModel project)
        {
            AddCommand = ReactiveCommand.Create(() => project.Plcs.Add(new Plc("PLC " + (project.Plcs.Count + 1))));
        }
    }
}
