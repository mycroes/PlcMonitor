using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using PlcMonitor.UI.DI;
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

        private GroupViewModel _root;
        public GroupViewModel Root
        {
            get => _root;
            set => this.RaiseAndSetIfChanged(ref _root, value);
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public PlcViewModel(IPlc plc, string name, IPlcInteractionManager plcInteractionManager,
            INotificationManager notificationManager, GroupViewModelFactory groupFactory)
        {
            _plcInteractionManager = plcInteractionManager;
            _notificationManager = notificationManager;

            Plc = plc;
            Name = name;
            _root = groupFactory.Invoke(this, string.Empty);
        }

        public VariableViewModel CreateVariable()
        {
            return _plcInteractionManager.CreateVariable(Plc);
        }

        public async Task Read(GroupViewModel group) {
            try
            {
                await _plcInteractionManager.Read(Plc, group.Variables);
            }
            catch (Exception e)
            {
                RxApp.MainThreadScheduler.Schedule(() => _notificationManager.Show(new Avalonia.Controls.Notifications.Notification(
                    "Failed to read from PLC", e.Message, NotificationType.Error)));
            }
        }
    }
}
