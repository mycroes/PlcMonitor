using System;
using System.Linq;
using PlcMonitor.UI.DI;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.Storage;
using PlcMonitor.UI.ViewModels;

namespace PlcMonitor.UI.Services
{
    public class MapperService : IMapperService
    {
        private readonly ProjectViewModelFactory _projectViewModelFactory;
        private readonly PlcViewModelFactory _plcViewModelFactory;

        public MapperService(ProjectViewModelFactory projectViewModelFactory, PlcViewModelFactory plcViewModelFactory)
        {
            _projectViewModelFactory = projectViewModelFactory;
            _plcViewModelFactory = plcViewModelFactory;
        }

        public ProjectViewModel MapFromStorage(Project project)
        {
            return _projectViewModelFactory.Invoke(project.Plcs.Select(MapFromStorage));
        }

        public Project MapToStorage(ProjectViewModel project)
        {
            return new Project(project.Plcs.Select(MapToStorage));
        }

        private PlcViewModel MapFromStorage(PlcConfiguration plc)
        {
            var res = _plcViewModelFactory.Invoke( plc.Plc, plc.Name, plc.Variables.Select(v => MapFromStorage(plc.Plc, v)) );

            return res;
        }

        private static VariableViewModel MapFromStorage(IPlc plc, Variable variable)
        {
            return variable switch
            {
                S7Variable s7var => new S7VariableViewModel((S7Plc) plc, variable.TypeCode, variable.Length, s7var.Address, MapFromStorage(variable.State)),
                _ => throw new Exception("Unsupported variable type")
            };
        }

        private static VariableStateViewModel? MapFromStorage(VariableState? state)
        {
            if (state == null) return null;

            return new VariableStateViewModel(state.Value, state.LastChange, state.LastRead);
        }

        private static PlcConfiguration MapToStorage(PlcViewModel plc)
        {
            return new PlcConfiguration(plc.Name, plc.Plc, plc.Variables.Select(MapToStorage));
        }

        private static Variable MapToStorage(VariableViewModel variable)
        {
            return variable switch
            {
                S7VariableViewModel s7var => new S7Variable(variable.TypeCode, variable.Length, s7var.Address, MapToStorage(variable.State)),
                _ => new Variable(variable.TypeCode, variable.Length, MapToStorage(variable.State))
            };
        }

        private static VariableState? MapToStorage(VariableStateViewModel? state)
        {
            if (state == null) return null;

            return new VariableState(state.Value, state.LastChange, state.LastRead.Value);
        }
    }
}