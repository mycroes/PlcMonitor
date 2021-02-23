using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using DynamicData.Binding;
using PlcMonitor.UI.Models.Plcs;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class PlcViewModel : ViewModelBase, IActivatableViewModel
    {
        private readonly IPlcInteractionManager _plcInteractionManager;
        private readonly INotificationManager _notificationManager;

        public string Name { get; }

        public IPlc Plc { get;}

        public ObservableCollectionExtended<VariableViewModel> Variables { get; } = new();

        public ReactiveCommand<Unit, VariableViewModel> AddCommand { get; }

        public ReactiveCommand<Unit, Unit> ReadCommand { get; }

        public ReactiveCommand<VariableViewModel, Unit> UpdateCommand { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public PlcViewModel(IPlc plc, string name, IPlcInteractionManager plcInteractionManager,
            INotificationManager notificationManager)
        {
            _plcInteractionManager = plcInteractionManager;
            _notificationManager = notificationManager;

            Plc = plc;
            Name = name;

            AddCommand = ReactiveCommand.Create<VariableViewModel>(Add);
            ReadCommand = ReactiveCommand.CreateFromTask(Read);
            UpdateCommand = ReactiveCommand.Create<VariableViewModel>(Update);

            this.WhenActivated(disposables => {
                ReadCommand.ThrownExceptions.ObserveOn(RxApp.MainThreadScheduler).Subscribe(e =>
                    _notificationManager.Show(new Avalonia.Controls.Notifications.Notification(
                        "Failed to read from PLC", e.Message, NotificationType.Error))).DisposeWith(disposables);
            });
        }

        public PlcViewModel(IPlc plc, string name, IEnumerable<VariableViewModel> variables,
            IPlcInteractionManager plcInteractionManager, INotificationManager notificationManager)
            : this(plc, name, plcInteractionManager, notificationManager)
        {
            Variables.AddRange(variables);
        }

        private VariableViewModel Add()
        {
            var variable = _plcInteractionManager.CreateVariable(Plc);
            Variables.Add(variable);

            return variable;
        }

        private Task Read() => _plcInteractionManager.Read(Plc, Variables);

        private void Update(VariableViewModel variable)
        {
            variable.Update();
        }
    }
}
