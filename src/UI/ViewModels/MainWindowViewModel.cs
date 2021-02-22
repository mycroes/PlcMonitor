using System.Collections.Generic;
using System.IO;
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
        public ReactiveCommand<Unit, Unit> SaveAsCommand { get; }

        public MainWindowViewModel(ProjectViewModel projectViewModel)
        {
            _project = projectViewModel;

            var canSave = this.WhenAnyValue(x => x.Project).Select(p => p.File is {});

            LoadCommand = ReactiveCommand.CreateFromTask(Load);
            SaveCommand = ReactiveCommand.CreateFromTask(() => Save(Project.File!), canSave);
            SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAs);
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

            var file = new FileInfo(fileNames[0]);
            var project = mapper.MapFromStorage(file, await storage.Load(file));
            Project = project;
        }

        private async Task Save(FileInfo file)
        {
            var mapper = Locator.Current.GetService<IMapperService>();
            var storage = Locator.Current.GetService<IStorageService>();

            var mapped = mapper.MapToStorage(Project);
            await storage.Save(mapped, file);
        }

        private async Task SaveAs()
        {
            var mainWindow = Locator.Current.GetService<MainWindow>();

            var dialog = new SaveFileDialog() {
                DefaultExtension = "plcson",
                Filters = GetFileFilters()
            };

            var fileName = await dialog.ShowAsync(mainWindow);
            if (fileName == null) return;

            var file = new FileInfo(fileName);
            await Save(file);
            Project.File = file;
        }

        private static List<FileDialogFilter> GetFileFilters()
        {
            return new() { new() { Name = "PlcMonitor files", Extensions = new() { "plcson" } } };
        }
    }
}
