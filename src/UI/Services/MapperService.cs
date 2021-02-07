using System.Linq;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.Storage;
using PlcMonitor.UI.ViewModels;

namespace PlcMonitor.UI.Services
{
    public class MapperService : IMapperService
    {
        public ProjectViewModel MapFromStorage(Project project)
        {
            return new ProjectViewModel(project.Plcs.Select(MapFromStorage));
        }

        public Project MapToStorage(ProjectViewModel project)
        {
            return new Project(project.Plcs.Select(MapToStorage));
        }

        private static PlcViewModel MapFromStorage(PlcConfiguration plc)
        {
            var res = new PlcViewModel ( plc.Plc, plc.Name, plc.Variables.Select(v => MapFromStorage(plc.Plc, v)) );

            return res;
        }

        private static VariableViewModel MapFromStorage(IPlc plc, Variable variable)
        {
            return new VariableViewModel(plc, variable.Address, variable.Type, variable.Length, MapFromStorage(variable.State));
        }

        private static VariableStateViewModel? MapFromStorage(VariableState? state)
        {
            if (state == null) return null;

            return new VariableStateViewModel(state.Value.Value, state.LastChange, state.LastRead);
        }

        private static PlcConfiguration MapToStorage(PlcViewModel plc)
        {
            return new PlcConfiguration(plc.Name, plc.Plc, plc.Variables.Select(MapToStorage));
        }

        private static Variable MapToStorage(VariableViewModel variable)
        {
            return new Variable(variable.Address, variable.Type, variable.Length, MapToStorage(variable.State));
        }

        private static VariableState? MapToStorage(VariableStateViewModel? state)
        {
            if (state == null) return null;

            return new VariableState(new ValueWithTypeCode(state.Value), state.LastChange, state.LastRead.Value);
        }
    }
}