using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.PlcData;
using PlcMonitor.UI.ViewModels;

public class PlcInteractionManager : IPlcInteractionManager
{
    public Task Read(IPlc plc, IEnumerable<VariableViewModel> variables)
    {
        foreach (var variable in variables)
        {
            variable.PushValue(new ReceivedValue(new Random().Next(3), DateTimeOffset.Now));
        }

        return Task.CompletedTask;
    }
}