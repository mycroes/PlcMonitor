using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class WriteVariableValueViewModel : ValidatableViewModelBase
    {
        public VariableViewModel Variable { get; }

        private object? _value;
        public object? Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public WriteVariableValueViewModel(VariableViewModel variable)
        {
            Variable = variable;
        }
    }
}