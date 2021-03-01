using System;

namespace PlcMonitor.UI.Models.Plcs
{
    public interface IPlc
    {
        IPlcConnection CreateConnection();

        IObservable<Connection> Connection { get; }
    }
}