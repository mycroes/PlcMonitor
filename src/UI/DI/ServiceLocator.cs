using System;
using System.Collections.Generic;
using System.Text.Json;
using PlcMonitor.UI.Infrastructure;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Services;
using PlcMonitor.UI.ViewModels;
using PlcMonitor.UI.ViewModels.Explorer;
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
            locator.Register<IMapperService>(() => new MapperService(Get<ProjectViewModelFactory>(), Get<PlcViewModelFactory>()));
            locator.Register<IPlcInteractionManager>(() => new PlcInteractionManager());

            locator.RegisterFactory<PlcViewModelFactory>((plc, name, variables) => new PlcViewModel(plc, name, variables, Get<IPlcInteractionManager>()));
            locator.RegisterFactory<AddConnectionNodeFactory>((project) => new AddConnectionNode(project, Get<PlcViewModelFactory>()));
            locator.RegisterFactory<ProjectViewModelFactory>((plcs) => new ProjectViewModel(plcs, Get<AddConnectionNodeFactory>()));
        }

        private static void RegisterFactory<TFactory>(this IMutableDependencyResolver locator, TFactory factory) where TFactory : Delegate
        {
            locator.RegisterLazySingleton<TFactory>(() => factory);
        }
    }

    public delegate PlcViewModel PlcViewModelFactory(IPlc plc, string name, IEnumerable<VariableViewModel> variables);
    public delegate AddConnectionNode AddConnectionNodeFactory(ProjectViewModel project);
    public delegate ProjectViewModel ProjectViewModelFactory(IEnumerable<PlcViewModel> plcs);
}