using System;

namespace PlcMonitor.UI.Models.PlcData
{
    public class ReceivedValue
    {
        public object Value { get; }
        public DateTimeOffset Timestamp { get; }

        public ReceivedValue(object value, DateTimeOffset timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }
    }
}
