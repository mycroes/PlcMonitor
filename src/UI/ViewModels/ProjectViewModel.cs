using System.Collections.ObjectModel;
using System.Reactive;
using DynamicData.Binding;
using PlcMonitor.UI.Models;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        public ObservableCollectionExtended<Plc> Plcs { get; } = new ObservableCollectionExtended<Plc>();

        public ReactiveCommand<Unit, Unit> AddCommand { get; }

        public ProjectViewModel()
        {
            AddCommand = ReactiveCommand.Create(() => Plcs.Add(new Plc("PLC " + (Plcs.Count + 1))));
        }
    }
}
