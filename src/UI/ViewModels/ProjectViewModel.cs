using DynamicData.Binding;
using PlcMonitor.UI.Models;

namespace PlcMonitor.UI.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        public ObservableCollectionExtended<IPlc> Plcs { get; } = new ObservableCollectionExtended<IPlc>();
    }
}
