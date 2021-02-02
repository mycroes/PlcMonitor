using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace PlcMonitor.UI.Views.Connection.Configuration
{
    public class S7ConnectionConfiguration : ReactiveUserControl<ViewModels.Connection.Configuration.S7ConnectionConfiguration>
    {
        public S7ConnectionConfiguration()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}