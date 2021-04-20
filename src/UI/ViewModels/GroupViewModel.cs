using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData.Binding;
using PlcMonitor.UI.DI;
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

        public ObservableCollectionExtended<VariableViewModel> SelectedVariables { get; } = new();

        public ReactiveCommand<Unit, VariableViewModel> AddCommand { get; }

        public ReactiveCommand<Unit, GroupViewModel> AddGroupCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public ReactiveCommand<Unit, Unit> ReadCommand { get; }

        public ReactiveCommand<VariableViewModel, Unit> UpdateCommand { get; }

        public ReactiveCommand<Unit, Unit> WriteCommand { get; }

        public GroupViewModel(PlcViewModel plc, string name, ShowDialog showDialog, GroupViewModelFactory groupFactory)
        {
            Plc = plc;
            _name = name;

            AddGroupCommand = ReactiveCommand.Create<GroupViewModel>(() => {
                var group = groupFactory.Invoke(Plc, string.Empty);
                SubGroups.Add(group);

                return group;
            });

            AddCommand = ReactiveCommand.Create<VariableViewModel>(Add);
            ReadCommand = ReactiveCommand.CreateFromTask(() => plc.Read(Variables));
            UpdateCommand = ReactiveCommand.Create<VariableViewModel>(Update);

            var hasSelection = SelectedVariables.WhenValueChanged(x => x.Count).Select(c => c > 0);
            DeleteCommand = ReactiveCommand.Create(() => Delete(SelectedVariables.ToList()), hasSelection);
            WriteCommand = ReactiveCommand.CreateFromTask(() => showDialog(new WriteViewModel(Plc, SelectedVariables)), hasSelection);
        }

        private VariableViewModel Add()
        {
            var variable = Plc.CreateVariable();
            Variables.Add(variable);

            return variable;
        }

        private void Delete(IEnumerable<VariableViewModel> variables)
        {
            foreach (var variable in variables)
            {
                Variables.Remove(variable);
            }
        }

        private void Update(VariableViewModel variable)
        {
            variable.Update();
        }
    }
}