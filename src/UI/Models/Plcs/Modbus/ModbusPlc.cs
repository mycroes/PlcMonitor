using System;

namespace PlcMonitor.UI.Models.Plcs.Modbus
{
    public class ModbusPlc : Plc
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

        public override IPlcConnection CreateConnection()
        {
            return new ModbusPlcConnection(Host, Port, UnitId);
        }

        protected override bool BreaksConnection(Exception exception)
        {
            return true;
        }
    }
}