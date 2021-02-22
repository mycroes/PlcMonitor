using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using PlcMonitor.UI.DI;
using PlcMonitor.UI.ViewModels;
using PlcMonitor.UI.Views;
using ReactiveUI;
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

                desktop.MainWindow = Locator.Current.GetService<MainWindow>();

                var notificationManager = Locator.Current.GetService<INotificationManager>();
                RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
                    RxApp.MainThreadScheduler.Schedule(() => notificationManager.Show(
                        new Avalonia.Controls.Notifications.Notification(
                            "Unhandled exception occured", ex.Message, NotificationType.Error))));

                desktop.MainWindow.DataContext = new MainWindowViewModel(
                    Locator.Current.GetService<ProjectViewModelFactory>().Invoke(null, Enumerable.Empty<PlcViewModel>()));
            }

            var theme = new Avalonia.Themes.Default.DefaultTheme();
            theme.TryGetResource("Button", out _);

            base.OnFrameworkInitializationCompleted();
        }
    }
}