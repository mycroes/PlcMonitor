using System;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData.Binding;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.PlcData;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class PlcViewModel : ViewModelBase
    {
        public string Name { get; }

        public IPlc Plc { get;}

        public ObservableCollectionExtended<VariableViewModel> Variables { get; } = new();

        public ReactiveCommand<Unit, Unit> ReadCommand { get; }

        public PlcViewModel(IPlc plc, string name)
        {
            Plc = plc;
            Name = name;

            Variables.Add(new VariableViewModel { Address = "DB1,INT2" });
            Variables.Add(new VariableViewModel { Address = "DB1,INT5" });

            ReadCommand = ReactiveCommand.CreateFromTask(Read);
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
