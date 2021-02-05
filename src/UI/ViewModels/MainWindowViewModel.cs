using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using PlcMonitor.UI.Services;
using PlcMonitor.UI.Views;
using ReactiveUI;
using Splat;

namespace PlcMonitor.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ProjectViewModel _project;
        public ProjectViewModel Project
        {
            get => _project;
            private set => this.RaiseAndSetIfChanged(ref _project, value);
        }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public MainWindowViewModel(ProjectViewModel projectViewModel)
        {
            _project = projectViewModel;

            LoadCommand = ReactiveCommand.CreateFromTask(Load);
            SaveCommand = ReactiveCommand.CreateFromTask(Save);
        }

        private async Task Load()
        {
            var mapper = Locator.Current.GetService<IMapperService>();
            var storage = Locator.Current.GetService<IStorageService>();
            var mainWindow = Locator.Current.GetService<MainWindow>();

            var dialog = new OpenFileDialog() {
                Filters = GetFileFilters(),
                AllowMultiple = false
            };

            var fileNames = await dialog.ShowAsync(mainWindow);
            if (fileNames?.FirstOrDefault() == null) return;

            var project = mapper.MapFromStorage(await storage.Load(fileNames[0]));
            Project = project;
        }

        private async Task Save()
        {
            var mapper = Locator.Current.GetService<IMapperService>();
            var storage = Locator.Current.GetService<IStorageService>();
            var mainWindow = Locator.Current.GetService<MainWindow>();

            var dialog = new SaveFileDialog() {
                DefaultExtension = ".plcson",
                Filters = GetFileFilters()
            };

            var fileName = await dialog.ShowAsync(mainWindow);
            if (fileName == null) return;

            await storage.Save(mapper.MapToStorage(Project), fileName);
        }

        private static List<FileDialogFilter> GetFileFilters()
        {
            return new() { new() { Name = "PlcMonitor files", Extensions = new() { ".plcson" } } };
        }
    }
}
