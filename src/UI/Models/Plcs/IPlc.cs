namespace PlcMonitor.UI.Models.Plcs
{
    public interface IPlc
    {
        IPlcConnection CreateConnection();
    }
}