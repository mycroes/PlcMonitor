using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using PlcMonitor.UI.Models;
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

                _variables.Columns.AddRange(GenerateColumns(ViewModel.Plc));
            });
        }

        private IEnumerable<DataGridColumn> GenerateColumns(IPlc plc)
        {
            DataGridTextColumn Col(string header, string path, bool readOnly = false)
            {
                return new DataGridTextColumn
                {
                    Header = header,
                    Binding = new Binding(path),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                    IsReadOnly = readOnly,
                };
            }

            switch (plc)
            {
                case S7Plc _:
                    yield return Col("Address", nameof(S7VariableViewModel.Address));
                    break;
                case ModbusPlc _:
                    yield return Col("Object type", nameof(ModbusVariableViewModel.ObjectType));
                    yield return Col("Address", nameof(ModbusVariableViewModel.Address));
                    break;
            }

            yield return Col("Length", nameof(VariableViewModel.Length));
            yield return Col("Type", nameof(VariableViewModel.TypeCode), true);
            yield return Col("Value", $"{nameof(VariableViewModel.State)}.{nameof(VariableViewModel.State.Value)}", true);
            yield return Col("Last change", $"{nameof(VariableViewModel.State)}.{nameof(VariableViewModel.State.LastChange)}", true);
            yield return Col("Last read", $"{nameof(VariableViewModel.State)}.{nameof(VariableViewModel.State.LastRead)}^");
        }
    }
}