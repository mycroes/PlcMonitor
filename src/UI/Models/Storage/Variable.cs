namespace PlcMonitor.UI.Models.Storage
{
    public class Variable
    {
        public string Address { get; }

        public string Type { get; }

        public int Length { get; }

        public VariableValue? Value { get; }

        public Variable(string address, string type, int length, VariableValue? value)
        {
            Address = address;
            Type = type;
            Length = length;
            Value = value;
        }
    }
}