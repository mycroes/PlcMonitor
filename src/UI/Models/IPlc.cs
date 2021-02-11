using System;

namespace PlcMonitor.UI.Models
{
    public interface IPlc
    {
        IPlcConnection CreateConnection();
    }
}