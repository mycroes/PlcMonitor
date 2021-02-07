namespace PlcMonitor.UI.Models
{
    public interface IPlc
    {
        IPlcConnection CreateConnection();

        bool IsValidAddress(string address);
    }
}