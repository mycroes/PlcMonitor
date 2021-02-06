namespace PlcMonitor.UI.Models.Storage
{
    public class Variable
    {
        public string Address { get; }

        public string Type { get; }

        public int Length { get; }

        public VariableState? State { get; }

        public Variable(string address, string type, int length, VariableState? state)
        {
            Address = address;
            Type = type;
            Length = length;
            State = state;
        }
    }
}