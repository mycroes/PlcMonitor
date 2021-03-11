using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using PlcMonitor.UI.DI;
using PlcMonitor.UI.Services;
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

                var viewModel = Locator.Current.GetService<MainWindowViewModel>();
                desktop.MainWindow.DataContext = viewModel;

                var args = Environment.GetCommandLineArgs().Skip(1).ToList();
                if (args.Count > 1)
                {
                    ShowCommandLineError($"Failed to parse command line arguments:\n\n{string.Join(" ", args)}", viewModel);
                }
                else if (args.Count == 1)
                {
                    var fi = new FileInfo(args[0]);
                    if (!fi.Exists)
                    {
                        ShowCommandLineError($"File '{fi.FullName}' does not exist.", viewModel);
                    }
                    else
                    {
                        OpenFile(fi.FullName, viewModel);
                    }
                }
            }

            var theme = new Avalonia.Themes.Default.DefaultTheme();
            theme.TryGetResource("Button", out _);

            base.OnFrameworkInitializationCompleted();
        }

        private async void ShowCommandLineError(string message, MainWindowViewModel viewModel)
        {
            await viewModel.ShowDialog(new DialogMessageViewModel(message));
        }

        private async void OpenFile(string fileName, MainWindowViewModel viewModel)
        {
            try
            {
                await viewModel.OpenFileCommand.Execute(fileName).FirstAsync();
            }
            catch (Exception e)
            {
                ShowCommandLineError($"Failed to open '{fileName}':\n\n{e.Message}", viewModel);
            }
        }
    }
}