using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using PlcMonitor.UI.DI;
using PlcMonitor.UI.Services;
using PlcMonitor.UI.Views;
using ReactiveUI;
using Splat;

namespace PlcMonitor.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
    {
        private readonly Subject<Unit> _projectPersisted = new Subject<Unit>();

        private readonly ProjectViewModelFactory _projectViewModelFactory;

        private ProjectViewModel _project;
        public ProjectViewModel Project
        {
            get => _project;
            private set => this.RaiseAndSetIfChanged(ref _project, value);
        }

        public ReactiveCommand<Unit, Unit> NewCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, bool> SaveAsCommand { get; }

        public IObservable<bool> HasChanges { get; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public MainWindowViewModel(ProjectViewModel? projectViewModel, ProjectViewModelFactory projectViewModelFactory)
        {
            _project = projectViewModel ?? projectViewModelFactory.Invoke(null, Enumerable.Empty<PlcViewModel>());
            _projectViewModelFactory = projectViewModelFactory;

            var canSave = this.WhenAnyValue(x => x.Project).Select(p => p.File is {});

            NewCommand = ReactiveCommand.Create(New);
            OpenCommand = ReactiveCommand.CreateFromTask(Open);
            SaveCommand = ReactiveCommand.CreateFromTask(() => Save(Project.File!), canSave);
            SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAs);

            HasChanges = Observable.Return(false)
                .Merge(this.WhenAnyValue(x => x.Project)
                    .SelectMany(x => x.Plcs.ToObservableChangeSet().TransformMany(p => p.Root.Variables).Select(_ => true)))
                .Merge(OpenCommand.Select(_ => false))
                .Merge(SaveCommand.Select(_ => false))
                .Merge(SaveAsCommand.Select(x => !x));
        }

        private void New()
        {
            Project = _projectViewModelFactory.Invoke(null, Enumerable.Empty<PlcViewModel>());
        }

        private async Task Open()
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

            _projectPersisted.OnNext(Unit.Default);
        }

        private async Task<bool> SaveAs()
        {
            var mainWindow = Locator.Current.GetService<MainWindow>();

            var dialog = new SaveFileDialog() {
                DefaultExtension = "plcson",
                Filters = GetFileFilters()
            };

            var fileName = await dialog.ShowAsync(mainWindow);
            if (fileName == null) return false;

            var file = new FileInfo(fileName);
            await Save(file);
            Project.File = file;

            return true;
        }

        private static List<FileDialogFilter> GetFileFilters()
        {
            return new() { new() { Name = "PlcMonitor files", Extensions = new() { "plcson" } } };
        }
    }
}
