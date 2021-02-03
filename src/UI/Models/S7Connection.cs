using System.Threading.Tasks;
using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.Models
{
    public class S7Connection : IPlcConnection
    {
        public Sally7.S7Connection Sally7Connection { get; }

        public S7Connection(string host, Tsap sourceTsap, Tsap destinationTsap)
        {
            Sally7Connection = new Sally7.S7Connection(host, sourceTsap, destinationTsap);
        }

        public async Task Open()
        {
            await Sally7Connection.OpenAsync();
        }

        public void Close()
        {
            Sally7Connection.Close();
        }
    }
}