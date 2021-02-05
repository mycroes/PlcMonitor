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
            return new ReceivedValue(value.RawValue, value.LastChange);
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
            // TODO storage value as JSON, include type
            return new VariableValue(SerializeValue(value.Value), value.Timestamp, value.Timestamp);
        }

        private static byte[] SerializeValue(object value)
        {
            return value switch
            {
                long v => BitConverter.GetBytes(v),
                ulong v => BitConverter.GetBytes(v),
                double v => BitConverter.GetBytes(v),
                int v => BitConverter.GetBytes(v),
                uint v => BitConverter.GetBytes(v),
                float v => BitConverter.GetBytes(v),
                short v => BitConverter.GetBytes(v),
                ushort v => BitConverter.GetBytes(v),
                byte v => new[] { v },
                sbyte v => new[] { unchecked((byte)v) },
                bool v => new byte[] { v ? 1 : 0 },
                string v => Encoding.UTF8.GetBytes(v),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}