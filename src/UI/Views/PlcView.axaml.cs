using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.ViewModels;
using ReactiveUI;

namespace PlcMonitor.UI.Views
{
    public class PlcView : ReactiveUserControl<PlcViewModel>
    {
        private readonly DataGrid _variables;

        public PlcView()
        {
            AvaloniaXamlLoader.Load(this);

            _variables = this.FindControl<DataGrid>(nameof(_variables));

            this.WhenActivated(disposables =>
            {
                Observable.FromEventPattern<DataGridRowEditEndedEventArgs>(_variables, nameof(_variables.RowEditEnded))
                    .Where(e => e.EventArgs.EditAction == DataGridEditAction.Commit)
                    .Select(e => (VariableViewModel)e.EventArgs.Row.DataContext!)
                    .SelectMany(v => ViewModel.UpdateCommand.Execute(v))
                    .Subscribe()
                    .DisposeWith(disposables);

                ViewModel.AddCommand.Subscribe(v => _variables.ScrollIntoView(v, null)).DisposeWith(disposables);
            });
        }
    }
}