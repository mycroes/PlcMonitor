using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels.Explorer;

namespace PlcMonitor.UI.Views
{
    public class PlcConnectionView : ReactiveUserControl<AddConnectionNode>
    {
        public PlcConnectionView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}