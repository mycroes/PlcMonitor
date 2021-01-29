using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;

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
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            ExtendClientAreaChromeHints =
                ExtendClientAreaChromeHints.PreferSystemChrome |
                ExtendClientAreaChromeHints.OSXThickTitleBar;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}