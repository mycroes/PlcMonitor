namespace PlcMonitor.UI.Models
{
    public class ModbusPlc : IPlc
    {
        public string Host { get; }
        public int Port { get; }
        public byte UnitId { get; }

        public ModbusPlc(string host, int port, byte unitId)
        {
            Host = host;
            Port = port;
            UnitId = unitId;
        }

        public IPlcConnection CreateConnection()
        {
            return new ModbusPlcConnection(Host, Port, UnitId);
        }
    }
}