namespace PlcMonitor.UI.Models
{
    public class ModbusPlc : IPlc
    {
        public string Name { get; }
        public string Host { get; }
        public int Port { get; }
        public byte UnitId { get; }

        public ModbusPlc(string name, string host, int port, byte unitId)
        {
            Name = name;
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