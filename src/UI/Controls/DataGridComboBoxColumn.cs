using System.Collections;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace PlcMonitor.UI.Controls
{
    public class DataGridComboBoxColumn : DataGridTextColumn
    {
        public DataGridComboBoxColumn()
        {
            typeof(DataGridTextColumn)
                .GetProperty("BindingTarget", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(this, ComboBox.SelectedItemProperty);
        }

        public static readonly DirectProperty<DataGridComboBoxColumn, IEnumerable> ItemsProperty =
            ComboBox.ItemsProperty.AddOwner<DataGridComboBoxColumn>(
                o => o.Items,
                (o, v) => o.Items = v
            );

        private IEnumerable _items = null!;
        public IEnumerable Items
        {
            get => _items;
            set => SetAndRaise(ItemsProperty, ref _items, value);
        }

        protected override void CancelCellEdit(IControl editingElement, object uneditedValue)
        {
            if (editingElement is ComboBox comboBox)
            {
                comboBox.SelectedIndex = uneditedValue as int? ?? -1;
            }
        }

        protected override IControl GenerateEditingElementDirect(DataGridCell cell, object dataItem)
        {
            var comboBox = new ComboBox
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Transparent)
            };

            SyncProperties(comboBox);

            return comboBox;
        }

        protected override object PrepareCellForEdit(IControl editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is ComboBox comboBox)
            {
                return comboBox.SelectedIndex;
            }

            return 0;
        }

        private void SyncProperties(AvaloniaObject content)
        {
            typeof(DataGridTextColumn)
                .GetMethod(nameof(SyncProperties), BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(this, new object[] { content });

            SyncColumnProperty(this, content, ItemsProperty);
        }

        private static void SyncColumnProperty<T>(AvaloniaObject column, AvaloniaObject content, AvaloniaProperty<T> property)
        {
            SyncColumnProperty(column, content, property, property);
        }

        private static void SyncColumnProperty<T>(AvaloniaObject column, AvaloniaObject content, AvaloniaProperty<T> contentProperty, AvaloniaProperty<T> columnProperty)
        {
            content.SetValue(contentProperty, column.GetValue(columnProperty));
        }
    }
}