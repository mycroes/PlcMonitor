using System.Reactive.Subjects;
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

        public BehaviorSubject<ReceivedValue?> Value { get; }

        public VariableViewModel() : this(string.Empty, string.Empty, 1, default)
        {
        }

        public VariableViewModel(string address, string type, int length, ReceivedValue? initialValue)
        {
            _address = address;
            _type = type;
            _length = length;
            Value = new BehaviorSubject<ReceivedValue?>(initialValue);
        }
    }
}
