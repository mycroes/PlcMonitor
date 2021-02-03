using System.Threading.Tasks;

namespace PlcMonitor.UI.Models
{
    public interface IPlcConnection
    {
        Task Open();
        void Close();
    }
}