using System.Net.Sockets;
using System.Threading.Tasks;
using NModbus;

namespace PlcMonitor.UI.Models.Plcs.Modbus
{
    public class ModbusPlcConnection : IPlcConnection
    {
        private readonly string _host;
        private readonly int _port;
        private readonly byte _unitId;

        public TcpClient TcpClient { get; }
        public IModbusMaster ModbusMaster { get; }

        public ModbusPlcConnection(string host, int port, byte unitId)
        {
            _host = host;
            _port = port;
            _unitId = unitId;

            TcpClient = new TcpClient();
            ModbusMaster = new ModbusFactory().CreateMaster(TcpClient);
        }

        public async Task Open()
        {
            await TcpClient.ConnectAsync(_host, _port);
        }

        public void Close()
        {
            TcpClient.Close();
        }
    }
}