using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Notifications;
using PlcMonitor.UI.Infrastructure;
using PlcMonitor.UI.Models.Plcs;
using PlcMonitor.UI.Services;
using PlcMonitor.UI.ViewModels;
using PlcMonitor.UI.ViewModels.Explorer;
using PlcMonitor.UI.Views;
using Splat;

namespace PlcMonitor.UI.DI
{
    public static class ServiceLocator
    {
        public static void Initialize(IMutableDependencyResolver locator, IReadonlyDependencyResolver resolver)
        {
            T Get<T>() => resolver.GetService<T>();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new PlcViewModelJsonConverter(),
                },
                WriteIndented = true
            };

            locator.Register<IStorageService>(() => new StorageService(jsonSerializerOptions));
            locator.Register<IMapperService>(() => new MapperService(Get<ProjectViewModelFactory>(), Get<PlcViewModelFactory>(), Get<GroupViewModelFactory>()));
            locator.Register<IPlcInteractionManager>(() => new PlcInteractionManager());

            locator.RegisterLazySingleton<MainWindowViewModel>(() => new MainWindowViewModel(null, Get<ProjectViewModelFactory>()));

            locator.RegisterFactory<PlcViewModelFactory>((plc, name) => new PlcViewModel(
                plc, name, Get<IPlcInteractionManager>(), Get<INotificationManager>(), Get<GroupViewModelFactory>()));

            locator.RegisterFactory<GroupViewModelFactory>((plc, name) => new GroupViewModel(
                plc, name, Get<ShowDialog>(), Get<GroupViewModelFactory>()
            ));

            locator.RegisterFactory<AddConnectionNodeFactory>((project) => new AddConnectionNode(project, Get<PlcViewModelFactory>()));
            locator.RegisterFactory<ProjectViewModelFactory>((file, plcs) => new ProjectViewModel(file, plcs, Get<AddConnectionNodeFactory>()));

            locator.RegisterLazySingleton<MainWindow>(() => new MainWindow());
            locator.RegisterLazySingleton<INotificationManager>(() => new WindowNotificationManager(resolver.GetService<MainWindow>())
            {
				Position = NotificationPosition.BottomRight,
				MaxItems = 4,
				Margin = new Thickness(0, 0, 15, 40)
			});

            locator.RegisterLazySingleton<ShowDialog>(() => Get<MainWindowViewModel>().ShowDialog);
        }

        private static void RegisterFactory<TFactory>(this IMutableDependencyResolver locator, TFactory factory) where TFactory : Delegate
        {
            locator.RegisterLazySingleton<TFactory>(() => factory);
        }
    }

    public delegate Task ShowDialog(IDialogContentViewModel content);
    public delegate PlcViewModel PlcViewModelFactory(IPlc plc, string name);
    public delegate GroupViewModel GroupViewModelFactory(PlcViewModel plc, string name);
    public delegate AddConnectionNode AddConnectionNodeFactory(ProjectViewModel project);
    public delegate ProjectViewModel ProjectViewModelFactory(FileInfo? file, IEnumerable<PlcViewModel> plcs);
}