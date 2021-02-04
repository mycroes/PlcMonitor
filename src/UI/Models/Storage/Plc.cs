using System.Collections.Generic;

namespace PlcMonitor.UI.Models.Storage
{
    public class Plc
    {
        public string Name { get; }
        public PlcType PlcType { get; }
        public IEnumerable<Variable> Variables { get; }

        public Plc(string name, PlcType plcType, IEnumerable<Variable> variables)
        {
            Name = name;
            PlcType = plcType;
            Variables = variables;
        }
    }
}