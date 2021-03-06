using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
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

        public async Task Read(IEnumerable<VariableViewModel> variables) {
            try
            {
                await _plcInteractionManager.Read(Plc, variables);
            }
            catch (Exception e)
            {
                RxApp.MainThreadScheduler.Schedule(() => _notificationManager.Show(new Avalonia.Controls.Notifications.Notification(
                    "Failed to read from PLC", e.Message, NotificationType.Error)));
            }
        }

        public async Task<bool> Write(WriteViewModel write)
        {
            try
            {
                await _plcInteractionManager.Write(Plc,
                    write.VariableValues.Where(v => v.Value != string.Empty).ToDictionary(v => v.Variable, v => v.ParsedValue));

                return true;
            }
            catch (Exception e)
            {
                RxApp.MainThreadScheduler.Schedule(() => _notificationManager.Show(new Avalonia.Controls.Notifications.Notification(
                    "Failed to write to PLC", e.Message, NotificationType.Error)));
            }

            return false;
        }
    }
}
