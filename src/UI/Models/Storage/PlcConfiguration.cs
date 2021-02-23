using System.Collections.Generic;
using PlcMonitor.UI.Models.Plcs;

namespace PlcMonitor.UI.Models.Storage
{
    public class PlcConfiguration
    {
        public string Name { get; }

        public IPlc Plc { get; }

        public IEnumerable<Variable> Variables { get; }

        public PlcConfiguration(string name, IPlc plc, IEnumerable<Variable> variables)
        {
            Name = name;
            Plc = plc;
            Variables = variables;
        }
    }
}