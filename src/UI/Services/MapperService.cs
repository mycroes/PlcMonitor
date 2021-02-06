using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlcMonitor.UI.Models.PlcData;
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
            return new PlcViewModel ( plc.Plc, plc.Name, plc.Variables.Select(MapFromStorage));
        }

        private static VariableViewModel MapFromStorage(Variable variable)
        {
            return new VariableViewModel(variable.Address, variable.Type, variable.Length, MapFromStorage(variable.Value));
        }

        private static ReceivedValue? MapFromStorage(VariableValue? value)
        {
            if (value == null) return null;

            // TODO fix value reading
            return new ReceivedValue(value.Value.Value, value.LastChange);
        }

        private static PlcConfiguration MapToStorage(PlcViewModel plc)
        {
            return new PlcConfiguration(plc.Name, plc.Plc, plc.Variables.Select(MapToStorage));
        }

        private static Variable MapToStorage(VariableViewModel variable)
        {
            return new Variable(variable.Address, variable.Type, variable.Length, MapToStorage(variable.Value.Value));
        }

        private static VariableValue? MapToStorage(ReceivedValue? value)
        {
            if (value == null) return null;

            // TODO implement last change
            return new VariableValue(SerializeValue(value.Value), value.Timestamp, value.Timestamp);
        }

        private static ValueWithTypeCode SerializeValue(object value)
        {
            var type = value.GetType();
            var isArray = type.IsArray;
            if (isArray) type = type.GetElementType()!;

            return new ValueWithTypeCode(Type.GetTypeCode(type), isArray, value);
        }
    }
}