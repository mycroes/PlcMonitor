using PlcMonitor.UI.Models.Plcs;

namespace PlcMonitor.UI.Models.Storage
{
    public class PlcConfiguration
    {
        public string Name { get; }

        public IPlc Plc { get; }

        public Group Root { get; }

        public PlcConfiguration(string name, IPlc plc, Group root)
        {
            Name = name;
            Plc = plc;
            Root = root;
        }
    }
}