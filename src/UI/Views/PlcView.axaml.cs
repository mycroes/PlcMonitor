using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels;

namespace PlcMonitor.UI.Views
{
    public class PlcView : ReactiveUserControl<PlcViewModel>
    {
        public PlcView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}