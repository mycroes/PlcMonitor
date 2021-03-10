using System.Collections.Generic;
using System.Threading.Tasks;
using Sally7;

namespace PlcMonitor.UI.Models.Plcs.S7
{
    public interface IS7PlcConnection : IPlcConnection
    {
        Task Read(IEnumerable<IDataItem> dataItems);
        Task Write(IEnumerable<IDataItem> dataItems);
    }
}