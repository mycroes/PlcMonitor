using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using PlcMonitor.UI.Models.Plcs;
using PlcMonitor.UI.ViewModels;
using ReactiveUI;

namespace PlcMonitor.UI.Controls
{
    public class PlcConnectionIndicator : ReactiveUserControl<PlcViewModel>
    {
        public PlcConnectionIndicator()
        {
            InitializeComponent();

            var c = this.FindControl<Path>("Circle");
            c.Transitions = new Avalonia.Animation.Transitions()
            {
                new SolidColorBrushTransition
                {
                    Property = Path.FillProperty,
                    Duration = TimeSpan.FromSeconds(1),
                }
            };

            this.WhenActivated(disposables => {
                this.WhenAnyValue(x => x.ViewModel.Plc)
                    .SelectMany(p => p.Connection)
                    .SelectMany(c => c.State)
                    .Select(s => s switch
                    {
                        ConnectionState.New => Colors.Gray,
                        ConnectionState.Opening => Colors.DodgerBlue,
                        ConnectionState.Open => Colors.Green,
                        ConnectionState.Closing => Colors.Red,
                        ConnectionState.Closed => Colors.Red,
                        _ => Colors.Orange
                    })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(c => new SolidColorBrush(c))
                    .BindTo(c, v => v.Fill)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel.Plc)
                    .SelectMany(p => p.Connection)
                    .SelectMany(c => c.State)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(state => c.SetValue(ToolTip.TipProperty, $"Connection state: {state}"))
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}