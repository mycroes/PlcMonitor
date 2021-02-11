using System.Collections.Generic;
using System.Threading.Tasks;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.ViewModels;

public interface IPlcInteractionManager
{
    Task Read(IPlc plc, IEnumerable<VariableViewModel> variables);
}
