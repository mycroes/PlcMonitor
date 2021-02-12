using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData.Binding;
using PlcMonitor.UI.Models;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class PlcViewModel : ViewModelBase
    {
        private readonly IPlcInteractionManager _plcInteractionManager;

        public string Name { get; }

        public IPlc Plc { get;}

        public ObservableCollectionExtended<VariableViewModel> Variables { get; } = new();

        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        public ReactiveCommand<Unit, Unit> ReadCommand { get; }

        public ReactiveCommand<VariableViewModel, Unit> UpdateCommand { get; }

        public PlcViewModel(IPlc plc, string name, IPlcInteractionManager plcInteractionManager)
        {
            _plcInteractionManager = plcInteractionManager;
            Plc = plc;
            Name = name;

            AddCommand = ReactiveCommand.Create(Add);
            ReadCommand = ReactiveCommand.CreateFromTask(Read);
            UpdateCommand = ReactiveCommand.Create<VariableViewModel>(Update);
        }

        public PlcViewModel(IPlc plc, string name, IEnumerable<VariableViewModel> variables, IPlcInteractionManager plcInteractionManager)
            : this(plc, name, plcInteractionManager)
        {
            Variables.AddRange(variables);
        }

        private void Add()
        {
            Variables.Add(_plcInteractionManager.CreateVariable(Plc));
        }

        private Task Read() => _plcInteractionManager.Read(Plc, Variables);

        private void Update(VariableViewModel variable)
        {
            variable.Update();
        }
    }
}
