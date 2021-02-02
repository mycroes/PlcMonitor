using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace PlcMonitor.UI.Views.Connection.Configuration
{
    public class ModbusConnectionConfiguration : ReactiveUserControl<ViewModels.Connection.Configuration.ModbusConnectionConfiguration>
    {
        public ModbusConnectionConfiguration()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}