using System.Text.Json;
using PlcMonitor.UI.Infrastructure;
using PlcMonitor.UI.Services;
using Splat;

namespace PlcMonitor.UI.DI
{
    public static class ServiceLocator
    {
        public static void Initialize(IMutableDependencyResolver locator, IReadonlyDependencyResolver resolver)
        {
            var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new PlcJsonConverter(), new TsapJsonConverter() }};

            locator.Register<IStorageService>(() => new StorageService(jsonSerializerOptions));
            locator.Register<IMapperService>(() => new MapperService());
        }
    }
}