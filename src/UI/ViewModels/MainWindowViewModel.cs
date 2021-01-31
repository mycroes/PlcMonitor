using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData.Binding;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.ViewModels.Explorer;
using ReactiveUI;

namespace PlcMonitor.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
    {
        public ObservableCollectionExtended<IExplorerNode> Nodes { get; } = new ObservableCollectionExtended<IExplorerNode>();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public OverviewNode OverviewNode { get; }
        public AddConnectionNode AddConnectionNode { get; }

        public ProjectViewModel ProjectViewModel { get; }

        public MainWindowViewModel(ProjectViewModel projectViewModel)
        {
            ProjectViewModel = projectViewModel;

            OverviewNode = new OverviewNode();
            AddConnectionNode = new AddConnectionNode(projectViewModel);

            BuildNodes();

            this.WhenActivated(disposables =>
            {
                projectViewModel.Plcs.ObserveCollectionChanges().Subscribe(e => {
                    if (e.EventArgs.OldItems is {} oldItems)
                        Nodes.RemoveRange(e.EventArgs.OldStartingIndex + 1, oldItems.Count);

                    if (e.EventArgs.NewItems is {} newItems)
                        Nodes.InsertRange(newItems.Cast<Plc>().Select(plc => new PlcConnectionNode(projectViewModel, plc)), e.EventArgs.NewStartingIndex + 1);
                }).DisposeWith(disposables);
            });
        }

        private void BuildNodes()
        {
            using (Nodes.SuspendNotifications())
            {
                Nodes.Clear();
                Nodes.Add(OverviewNode);
                Nodes.AddRange(ProjectViewModel.Plcs.Select(plc => new PlcConnectionNode(ProjectViewModel, plc)));
                Nodes.Add(AddConnectionNode);
            }
        }
    }
}
