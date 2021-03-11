using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels;

namespace PlcMonitor.UI.Views
{
    public class DialogMessageView : ReactiveUserControl<WriteViewModel>
    {
        public DialogMessageView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}