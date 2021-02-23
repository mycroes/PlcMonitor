using System.Threading.Tasks;

namespace PlcMonitor.UI.Models.Plcs
{
    public interface IPlcConnection
    {
        Task Open();
        void Close();
    }
}