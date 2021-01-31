using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels.Explorer;

namespace PlcMonitor.UI.Views
{
    public class AddConnectionView : ReactiveUserControl<AddConnectionNode>
    {
        public AddConnectionView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}