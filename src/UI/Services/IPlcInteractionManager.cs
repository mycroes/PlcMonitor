using System.Collections.Generic;
using System.Threading.Tasks;
using PlcMonitor.UI.Models.Plcs;
using PlcMonitor.UI.ViewModels;

public interface IPlcInteractionManager
{
    VariableViewModel CreateVariable(IPlc plc);

    Task Read(IPlc plc, IEnumerable<VariableViewModel> variables);

    Task Write(IPlc plc, IDictionary<VariableViewModel, object?> variableValues);
}
