using System.Reactive.Disposables;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels;
using ReactiveUI;

namespace PlcMonitor.UI.Views
{
    public class ProjectView : ReactiveUserControl<ProjectViewModel>
    {
        public ProjectView()
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