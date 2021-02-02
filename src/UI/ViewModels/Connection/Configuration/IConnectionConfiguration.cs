using PlcMonitor.UI.Models;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;

namespace PlcMonitor.UI.ViewModels.Connection.Configuration
{
    public interface IConnectionConfiguration : IReactiveObject, IValidatableViewModel
    {
        string Title { get; }
        IPlc CreatePlc();
    }
}