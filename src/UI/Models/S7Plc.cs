using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.Models
{
    public class S7Plc : IPlc
    {
        public string Host { get; }
        public Tsap SourceTsap { get; }
        public Tsap DestinationTsap { get; }

        public S7Plc(string host, Tsap sourceTsap, Tsap destinationTsap)
        {
            Host = host;
            SourceTsap = sourceTsap;
            DestinationTsap = destinationTsap;
        }

        public IPlcConnection CreateConnection()
        {
            return new S7Connection(Host, SourceTsap, DestinationTsap);
        }
    }
}