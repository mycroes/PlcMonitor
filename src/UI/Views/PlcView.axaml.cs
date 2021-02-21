using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using PlcMonitor.UI.Controls;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.Modbus;
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
                    yield return Col("Length", nameof(VariableViewModel.Length));
                    yield return Col("Type", nameof(VariableViewModel.TypeCode), true);
                    break;
                case ModbusPlc _:
                    yield return new DataGridComboBoxColumn
                    {
                        Header = "Object type",
                        Binding = new Binding(nameof(ModbusVariableViewModel.ObjectType)),
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        IsReadOnly = false,
                        Items = new[] {
                            ObjectType.Coil,
                            ObjectType.DiscreteInput,
                            ObjectType.HoldingRegister,
                            ObjectType.InputRegister
                        }
                    };
                    yield return Col("Address", nameof(ModbusVariableViewModel.Address));
                    yield return Col("Length", nameof(VariableViewModel.Length));
                    // Addressing is at 16 bit register level only, so we allow manual typecode selection
                    yield return new DataGridComboBoxColumn
                    {
                        Header = "Type",
                        Binding = new Binding(nameof(VariableViewModel.TypeCode)),
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        IsReadOnly = false,
                        Items = new[] {
                            TypeCode.Boolean,
                            TypeCode.Byte,
                            TypeCode.Double,
                            TypeCode.Int16,
                            TypeCode.Int32,
                            TypeCode.Int64,
                            TypeCode.SByte,
                            TypeCode.Single,
                            TypeCode.String,
                            TypeCode.UInt16,
                            TypeCode.UInt32,
                            TypeCode.UInt64
                        }
                    };
                    break;
            }

            yield return Col("Value", $"{nameof(VariableViewModel.State)}.{nameof(VariableViewModel.State.Value)}", true);
            yield return Col("Last change", $"{nameof(VariableViewModel.State)}.{nameof(VariableViewModel.State.LastChange)}", true);
            yield return Col("Last read", $"{nameof(VariableViewModel.State)}.{nameof(VariableViewModel.State.LastRead)}^", true);
        }
    }
}