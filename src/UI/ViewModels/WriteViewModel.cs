using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class WriteViewModel : ViewModelBase, IDialogContentViewModel
    {

        public IObservable<Unit> Close => CloseCommand;
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public ReactiveCommand<Unit, Unit> WriteCommand { get; }

        public PlcViewModel Plc { get; }
        public ObservableCollectionExtended<WriteVariableValueViewModel> VariableValues { get; }

        private bool _readWritten = true;
        public bool ReadWritten
        {
            get => _readWritten;
            set => this.RaiseAndSetIfChanged(ref _readWritten, value);
        }

        public WriteViewModel(PlcViewModel plc, IEnumerable<VariableViewModel> variables)
        {
            Plc = plc;
            VariableValues = new ObservableCollectionExtended<WriteVariableValueViewModel>(
                variables.Select(v => new WriteVariableValueViewModel(plc, v)));

            CloseCommand = ReactiveCommand.Create(() => {});

            WriteCommand = ReactiveCommand.CreateFromTask(async () => {
                await plc.Write(this);
                if (ReadWritten) await plc.Read(VariableValues.Select(x => x.Variable));

                await CloseCommand.Execute();
            });
        }
    }
}