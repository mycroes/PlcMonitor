using System;
using System.Reactive.Subjects;

namespace PlcMonitor.UI.ViewModels
{
    public class VariableStateViewModel
    {
        public object Value { get; }

        public DateTimeOffset LastChange { get; }

        public BehaviorSubject<DateTimeOffset> LastRead { get; }

        public VariableStateViewModel(object value, DateTimeOffset lastChange, DateTimeOffset lastRead)
        {
            Value = value;
            LastChange = lastChange;
            LastRead = new BehaviorSubject<DateTimeOffset>(lastRead);
        }
    }
}
