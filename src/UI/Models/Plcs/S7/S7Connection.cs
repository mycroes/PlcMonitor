using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sally7;
using Sally7.Protocol.Cotp;

namespace PlcMonitor.UI.Models.Plcs.S7
{
    public class S7Connection : IS7PlcConnection
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

        public Task Read(IEnumerable<IDataItem> dataItems)
        {
            return Sally7Connection.ReadAsync(dataItems.ToArray());
        }
    }
}