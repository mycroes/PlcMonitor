using System;
using System.Linq;
using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.Models.Plcs.S7
{
    public class S7Plc : Plc
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

        public override IS7PlcConnection CreateConnection()
        {
            return new S7Connection(Host, SourceTsap, DestinationTsap);
        }

        public bool IsValidAddress(string address)
        {
            return !string.IsNullOrWhiteSpace(address) && S7.AddressParser.TryParse(address, out _, out _, out _, out _);
        }

        protected override bool BreaksConnection(Exception exception)
        {
            return !IsDataItemError(exception);
        }

        private bool IsDataItemError(Exception exception)
        {
            // Bug in Sally7, write exceptions also start with 'Read of dataItem'
            return exception.Message.StartsWith("Read of dataItem")
                || exception is AggregateException ae
                && ae.InnerExceptions.All(BreaksConnection);
        }
    }
}