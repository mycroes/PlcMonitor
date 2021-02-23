using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace PlcMonitor.UI.Models.Plcs
{
    public class Connection
    {
        public BehaviorSubject<ConnectionState> State { get; } = new BehaviorSubject<ConnectionState>(ConnectionState.New);
        public DateTimeOffset? Opened { get; }
        public DateTimeOffset? Closed { get; }
        public bool IsConnected { get; }

        public IPlcConnection PlcConnection { get; }

        public Connection(IPlcConnection plcConnection)
        {
            PlcConnection = plcConnection;
        }

        public async Task Open()
        {
            State.OnNext(ConnectionState.Opening);
            try {
                await PlcConnection.Open();
                State.OnNext(ConnectionState.Open);
            }
            catch
            {
                State.OnNext(ConnectionState.Closed);
                throw;
            }
        }

        public void Close()
        {
            State.OnNext(ConnectionState.Closing);
            try {
                PlcConnection.Close();
            }
            finally
            {
                State.OnNext(ConnectionState.Closed);
            }
        }
    }
}