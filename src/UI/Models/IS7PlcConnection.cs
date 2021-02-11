using System.Collections.Generic;
using System.Threading.Tasks;
using Sally7;

namespace PlcMonitor.UI.Models
{
    public interface IS7PlcConnection : IPlcConnection
    {
        Task Read(IEnumerable<IDataItem> dataItems);
    }
}