using System;
using System.IO;
using System.Linq;
using PlcMonitor.UI.DI;
using PlcMonitor.UI.Models.Plcs;
using PlcMonitor.UI.Models.Plcs.Modbus;
using PlcMonitor.UI.Models.Plcs.S7;
using PlcMonitor.UI.Models.Storage;
using PlcMonitor.UI.ViewModels;

namespace PlcMonitor.UI.Services
{
    public class MapperService : IMapperService
    {
        private readonly ProjectViewModelFactory _projectViewModelFactory;
        private readonly PlcViewModelFactory _plcViewModelFactory;
        private readonly GroupViewModelFactory _groupViewModelFactory;

        public MapperService(ProjectViewModelFactory projectViewModelFactory, PlcViewModelFactory plcViewModelFactory,
            GroupViewModelFactory groupViewModelFactory)
        {
            _projectViewModelFactory = projectViewModelFactory;
            _plcViewModelFactory = plcViewModelFactory;
            _groupViewModelFactory = groupViewModelFactory;
        }

        public ProjectViewModel MapFromStorage(FileInfo file, Project project)
        {
            return _projectViewModelFactory.Invoke(file, project.Plcs.Select(MapFromStorage));
        }

        public Project MapToStorage(ProjectViewModel project)
        {
            return new Project(project.Plcs.Select(MapToStorage));
        }

        private PlcViewModel MapFromStorage(PlcConfiguration plc)
        {
            var res = _plcViewModelFactory.Invoke( plc.Plc, plc.Name );
            res.Root = MapFromStorage(res, plc.Root);

            return res;
        }

        private GroupViewModel MapFromStorage(PlcViewModel plc, Group group)
        {
            var vm = _groupViewModelFactory.Invoke(plc, group.Name);
            vm.SubGroups.AddRange(group.SubGroups.Select(g => MapFromStorage(plc, g)));
            vm.Variables.AddRange(group.Variables.Select(v => MapFromStorage(plc.Plc, v)));

            return vm;
        }

        private static VariableViewModel MapFromStorage(IPlc plc, Variable variable)
        {
            return variable switch
            {
                S7Variable s7var => new S7VariableViewModel((S7Plc)plc, variable.Name, s7var.Address, variable.TypeCode,
                    variable.Length, MapFromStorage(variable.State)),

                ModbusVariable modbusVar => new ModbusVariableViewModel((ModbusPlc)plc, variable.Name,
                    modbusVar.ObjectType, modbusVar.Address, variable.TypeCode, variable.Length,
                    MapFromStorage(variable.State)),

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
            return new PlcConfiguration(plc.Name, plc.Plc, MapToStorage(plc.Root));
        }

        private static Group MapToStorage(GroupViewModel group)
        {
            return new Group(group.Name, group.SubGroups.Select(MapToStorage),
                group.Variables.Select(MapToStorage));
        }

        private static Variable MapToStorage(VariableViewModel variable)
        {
            return variable switch
            {
                S7VariableViewModel s7var => new S7Variable(variable.Name, s7var.Address, variable.TypeCode,
                    variable.Length, MapToStorage(variable.State)),

                ModbusVariableViewModel modbusVar => new ModbusVariable(variable.Name, modbusVar.ObjectType,
                    modbusVar.Address, variable.TypeCode, variable.Length, MapToStorage(variable.State)),

                _ => throw new ArgumentException($"Unsupport variable type given: {variable}.")
            };
        }

        private static VariableState? MapToStorage(VariableStateViewModel? state)
        {
            if (state == null) return null;

            return new VariableState(state.Value, state.LastChange, state.LastRead.Value);
        }
    }
}