using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PlcMonitor.UI.DI;
using PlcMonitor.UI.ViewModels;
using PlcMonitor.UI.Views;
using Splat;

namespace PlcMonitor.UI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                ServiceLocator.Initialize(Locator.CurrentMutable, Locator.Current);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(new ProjectViewModel()),
                };

                Locator.CurrentMutable.RegisterConstant((MainWindow) desktop.MainWindow);
            }

            var theme = new Avalonia.Themes.Default.DefaultTheme();
            theme.TryGetResource("Button", out _);

            base.OnFrameworkInitializationCompleted();
        }
    }
}