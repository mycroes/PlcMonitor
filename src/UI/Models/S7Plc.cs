using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.Models
{
    public class S7Plc : IPlc
    {
        public string Name { get; }
        public string Host { get; }
        public Tsap SourceTsap { get; }
        public Tsap DestinationTsap { get; }

        public S7Plc(string name, string host, Tsap sourceTsap, Tsap destinationTsap)
        {
            Name = name;
            Host = host;
            SourceTsap = sourceTsap;
            DestinationTsap = destinationTsap;
        }
    }
}