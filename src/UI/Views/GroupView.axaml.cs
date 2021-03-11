using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using PlcMonitor.UI.Controls;
using PlcMonitor.UI.Models.Plcs;
using PlcMonitor.UI.Models.Plcs.Modbus;
using PlcMonitor.UI.Models.Plcs.S7;
using PlcMonitor.UI.ValueConverters;
using PlcMonitor.UI.ViewModels;
using ReactiveUI;

namespace PlcMonitor.UI.Views
{
    public class GroupView : ReactiveUserControl<GroupViewModel>
    {
        private readonly DataGrid _variables;

        public GroupView()
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

                Observable.FromEventPattern<SelectionChangedEventArgs>(_variables, nameof(_variables.SelectionChanged))
                    .Select(e => e.EventArgs.AddedItems.Cast<VariableViewModel>())
                    .Subscribe(items => ViewModel.SelectedVariables.AddRange(items))
                    .DisposeWith(disposables);

                Observable.FromEventPattern<SelectionChangedEventArgs>(_variables, nameof(_variables.SelectionChanged))
                    .Select(e => e.EventArgs.RemovedItems.Cast<VariableViewModel>())
                    .Subscribe(items => ViewModel.SelectedVariables.RemoveMany(items))
                    .DisposeWith(disposables);

                _variables.Columns.AddRange(GenerateColumns(ViewModel.Plc.Plc));
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

            yield return Col("Name", nameof(VariableViewModel.Name));

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

            yield return new DataGridTextColumn
            {
                Header = "Value",
                Binding = new Binding("State.Value")
                {
                    Mode = BindingMode.OneWay,
                    Converter = new ArrayJoinConverter()
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                IsReadOnly = true,
            };

            yield return Col("Last change", "State.LastChange.DateTime", true);
            yield return Col("Last read", "State.LastRead^.DateTime", true);
        }
    }
}