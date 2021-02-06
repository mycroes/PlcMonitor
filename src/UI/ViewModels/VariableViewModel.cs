using System.Collections;
using PlcMonitor.UI.Models.PlcData;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class VariableViewModel : ValidatableViewModelBase
    {
        private string _address;
        public string Address
        {
            get => _address;
            set => this.RaiseAndSetIfChanged(ref _address, value);
        }

        private string _type;
        public string Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }

        private int _length;
        public int Length
        {
            get => _length;
            set => this.RaiseAndSetIfChanged(ref _length, value);
        }

        private VariableStateViewModel? _state;
        public VariableStateViewModel? State
        {
            get => _state;
            set => this.RaiseAndSetIfChanged(ref _state, value);
        }

        public VariableViewModel() : this(string.Empty, string.Empty, 1, default)
        {
        }

        public VariableViewModel(string address, string type, int length, VariableStateViewModel? state)
        {
            _address = address;
            _type = type;
            _length = length;
            _state = state;
        }

        public void PushValue(ReceivedValue next)
        {
            if (State != null && StructuralComparisons.StructuralEqualityComparer.Equals(next.Value, State.Value))
            {
                State.LastRead.OnNext(next.Timestamp);
            }
            else
            {
                State = new VariableStateViewModel(next.Value, next.Timestamp, next.Timestamp);
            }
        }
    }
}
