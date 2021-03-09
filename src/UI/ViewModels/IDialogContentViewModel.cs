using System;
using System.Reactive;

namespace PlcMonitor.UI.ViewModels
{
    public interface IDialogContentViewModel
    {
        IObservable<Unit> Close { get; }
    }
}
