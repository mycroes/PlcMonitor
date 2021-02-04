using System;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData.Binding;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.PlcData;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels.Explorer
{
    public class PlcConnectionNode : IExplorerNode
    {
        private readonly ProjectViewModel _project;
        public IPlc Plc { get; }

        public string Name => Plc.Name;

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public ReactiveCommand<Unit, Unit> ReadCommand { get; }

        public ObservableCollectionExtended<VariableViewModel> Variables { get; } = new();

        public PlcConnectionNode(ProjectViewModel project, IPlc plc)
        {
            _project = project;
            Plc = plc;

            DeleteCommand = ReactiveCommand.Create(() => { project.Plcs.Remove(plc); });
            ReadCommand = ReactiveCommand.CreateFromTask(Read);

            Variables.Add(new VariableViewModel { Address = "DB1,INT2" });
            Variables.Add(new VariableViewModel { Address = "DB1,INT5" });
        }

        private Task Read()
        {
            foreach (var variable in Variables)
            {
                variable.Value.OnNext(new ReceivedValue(new Random().Next(), DateTimeOffset.Now));
            }

            return Task.CompletedTask;
        }
    }
}
