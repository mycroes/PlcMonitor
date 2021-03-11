using System;
using System.Reactive;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class DialogMessageViewModel : ViewModelBase, IDialogContentViewModel
    {
        public IObservable<Unit> Close => CloseCommand;
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public string Message { get; }

        public DialogMessageViewModel(string message)
        {
            Message = message;

            CloseCommand = ReactiveCommand.Create(() => {});
        }
    }
}