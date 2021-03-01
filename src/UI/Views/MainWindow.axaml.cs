using System;
using System.IO;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using PlcMonitor.UI.ViewModels;
using ReactiveUI;

namespace PlcMonitor.UI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaTitleBarHeightHint = -1;

            TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;

            this.GetObservable(IsExtendedIntoWindowDecorationsProperty)
                .Subscribe(x =>
                {
                    if (!x)
                    {
                        SystemDecorations = SystemDecorations.Full;
                        TransparencyLevelHint = WindowTransparencyLevel.Blur;
                    }
                });

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenAnyValue(x => x.DataContext)
                .Select(x => x as MainWindowViewModel)
                .Where(x => x != null)
                .Select(x => x!)
                .SelectMany(x => x.WhenAnyValue(vm => vm.Project.File))
                .Select(CreateTitle)
                .BindTo(this, v => v.Title);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            ExtendClientAreaChromeHints =
                ExtendClientAreaChromeHints.PreferSystemChrome |
                ExtendClientAreaChromeHints.OSXThickTitleBar;
        }

        private string CreateTitle(FileInfo? file)
        {
            var fn = file == null
                ? null
                : file.Name.EndsWith(".plcson")
                    ? file.Name.Substring(0, file.Name.Length - 7)
                    : file.Name;

            return fn == null ? "PlcMonitor" : $"{fn} - PlcMonitor";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}