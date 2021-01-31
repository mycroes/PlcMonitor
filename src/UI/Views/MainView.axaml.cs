using System.Reactive.Disposables;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels;
using ReactiveUI;

namespace PlcMonitor.UI.Views
{
    public class MainView : ReactiveUserControl<MainWindowViewModel>
    {
        public MainView()
        {
            this.WhenActivated(disposables => Disposable.Create(() => { }).DisposeWith(disposables));

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}