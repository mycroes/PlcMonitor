using PlcMonitor.UI.Models;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PlcMonitor.UI.ViewModels.Connection.Configuration
{
    public abstract class ConnectionConfigurationBase : ValidatableViewModelBase
    {
        private string? _name;
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public abstract string Title { get; }

        protected ConnectionConfigurationBase()
        {
            this.ValidationRule(x => x.Name, x => !string.IsNullOrWhiteSpace(x), "Name must be set");
        }

        public abstract IPlc CreatePlc();
    }
}