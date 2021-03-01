using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels;
using ReactiveUI;

namespace PlcMonitor.UI.Views
{
    public class MainView : ReactiveUserControl<MainWindowViewModel>
    {
        private readonly TextBlock _title;

        public MainView()
        {
            InitializeComponent();

            _title = this.FindControl<TextBlock>(nameof(_title));

            this.WhenActivated(disposables => {
                this.ViewModel.HasChanges.CombineLatest(this.ViewModel.WhenAnyValue(vm => vm.Project.File), CreateTitle).BindTo(this, v => v._title.Text).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private string CreateTitle(bool changes, FileInfo? file)
        {
            var res = changes ? "• " : "";

            var title = file == null
                ? "Untitled"
                : file.Name.EndsWith(".plcson")
                    ? file.Name.Substring(0, file.Name.Length - 7)
                    : file.Name;

            return res + title + " - PlcMonitor";
        }
    }
}