using System.Collections.Generic;

namespace PlcMonitor.UI.Models.Storage
{
    public class Project
    {
        public IEnumerable<PlcConfiguration> Plcs { get; }

        public Project(IEnumerable<PlcConfiguration> plcs)
        {
            Plcs = plcs;
        }
    }
}