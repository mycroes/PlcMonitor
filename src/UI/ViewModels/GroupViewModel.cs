using System.Reactive;
using DynamicData.Binding;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class GroupViewModel : ViewModelBase
    {
        public PlcViewModel Plc { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public ObservableCollectionExtended<GroupViewModel> SubGroups { get; } = new();

        public ObservableCollectionExtended<VariableViewModel> Variables { get; } = new();

        public ReactiveCommand<Unit, VariableViewModel> AddCommand { get; }

        public ReactiveCommand<Unit, GroupViewModel> AddGroupCommand { get; }

        public ReactiveCommand<Unit, Unit> ReadCommand { get; }

        public ReactiveCommand<VariableViewModel, Unit> UpdateCommand { get; }

        public GroupViewModel(PlcViewModel plc, string name)
        {
            Plc = plc;
            _name = name;

            AddGroupCommand = ReactiveCommand.Create<GroupViewModel>(() => {
                var group = new GroupViewModel(Plc, string.Empty);
                SubGroups.Add(group);

                return group;
            });

            AddCommand = ReactiveCommand.Create<VariableViewModel>(Add);
            ReadCommand = ReactiveCommand.CreateFromTask(() => plc.Read(this));
            UpdateCommand = ReactiveCommand.Create<VariableViewModel>(Update);
        }

        private VariableViewModel Add()
        {
            var variable = Plc.CreateVariable();
            Variables.Add(variable);

            return variable;
        }

        private void Update(VariableViewModel variable)
        {
            variable.Update();
        }
    }
}