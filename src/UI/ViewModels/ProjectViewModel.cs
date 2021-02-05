using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using DynamicData.Binding;
using PlcMonitor.UI.ViewModels.Explorer;
using ReactiveUI;
using System.Collections.Generic;

namespace PlcMonitor.UI.ViewModels
{
    public class ProjectViewModel : ViewModelBase, IActivatableViewModel
    {
        public ObservableCollectionExtended<IExplorerNode> Nodes { get; } = new ObservableCollectionExtended<IExplorerNode>();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public OverviewNode OverviewNode { get; }
        public AddConnectionNode AddConnectionNode { get; }

        public ObservableCollectionExtended<PlcViewModel> Plcs { get; } = new ObservableCollectionExtended<PlcViewModel>();

        public ProjectViewModel()
        {
            OverviewNode = new OverviewNode();
            AddConnectionNode = new AddConnectionNode(this);

            this.WhenActivated(disposables =>
            {
                BuildNodes();

                Plcs.ObserveCollectionChanges().Subscribe(e =>
                {
                    if (e.EventArgs.OldItems is { } oldItems)
                        Nodes.RemoveRange(e.EventArgs.OldStartingIndex + 1, oldItems.Count);

                    if (e.EventArgs.NewItems is { } newItems)
                        Nodes.InsertRange(newItems.Cast<PlcViewModel>().Select(plc => new PlcConnectionNode(this, plc)), e.EventArgs.NewStartingIndex + 1);
                }).DisposeWith(disposables);
            });
        }

        public ProjectViewModel(IEnumerable<PlcViewModel> plcs)
            : this()
        {
            Plcs.AddRange(plcs);
        }

        private void BuildNodes()
        {
            using (Nodes.SuspendNotifications())
            {
                Nodes.Clear();
                Nodes.Add(OverviewNode);
                Nodes.AddRange(Plcs.Select(plc => new PlcConnectionNode(this, plc)));
                Nodes.Add(AddConnectionNode);
            }
        }
    }
}
