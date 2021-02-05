using DynamicData.Binding;

namespace PlcMonitor.UI.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {
        public ObservableCollectionExtended<PlcViewModel> Plcs { get; } = new ObservableCollectionExtended<PlcViewModel>();
    }
}
