using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels;

namespace PlcMonitor.UI.Views
{
    public class WriteView : ReactiveUserControl<WriteViewModel>
    {
        public WriteView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}