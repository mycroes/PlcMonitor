using System;
using System.Collections.Generic;
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

            ReadCommand = ReactiveCommand.CreateFromTask(Read);
        }

        public PlcViewModel(IPlc plc, string name, IEnumerable<VariableViewModel> variables)
            : this(plc, name)
        {
            Variables.AddRange(variables);
        }

        private Task Read()
        {
            foreach (var variable in Variables)
            {
                variable.PushValue(new ReceivedValue(new Random().Next(3), DateTimeOffset.Now));
            }

            return Task.CompletedTask;
        }
    }
}
