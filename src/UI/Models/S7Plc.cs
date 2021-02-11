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

        IPlcConnection IPlc.CreateConnection() => CreateConnection();

        public IS7PlcConnection CreateConnection()
        {
            return new S7Connection(Host, SourceTsap, DestinationTsap);
        }

        public bool IsValidAddress(string address)
        {
            return !string.IsNullOrWhiteSpace(address) && S7.AddressParser.TryParse(address, out _, out _, out _, out _);
        }
    }
}