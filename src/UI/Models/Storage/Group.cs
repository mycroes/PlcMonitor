using System.Collections.Generic;

namespace PlcMonitor.UI.Models.Storage
{
    public class Group
    {
        public string Name { get; }
        public IEnumerable<Group> SubGroups { get; }
        public IEnumerable<Variable> Variables { get; }

        public Group(string name, IEnumerable<Group> subGroups, IEnumerable<Variable> variables)
        {
            Name = name;
            SubGroups = subGroups;
            Variables = variables;
        }
    }
}