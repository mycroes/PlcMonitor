using System.Collections.Generic;

namespace PlcMonitor.UI.Models.Storage
{
    public class Project
    {
        public IEnumerable<Plc> Plcs { get; }

        public Project(IEnumerable<Plc> plcs)
        {
            Plcs = plcs;
        }
    }
}