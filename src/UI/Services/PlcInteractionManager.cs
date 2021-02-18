using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlcMonitor.UI.Models;
using PlcMonitor.UI.Models.PlcData;
using PlcMonitor.UI.Models.S7;
using PlcMonitor.UI.ViewModels;
using Sally7;

public class PlcInteractionManager : IPlcInteractionManager
{
    public VariableViewModel CreateVariable(IPlc plc)
    {
        return plc switch
        {
            S7Plc s7plc => new S7VariableViewModel(s7plc),
            ModbusPlc modbusPlc => new ModbusVariableViewModel(modbusPlc),
            _ => throw new NotImplementedException()
        };
    }

    public Task Read(IPlc plc, IEnumerable<VariableViewModel> variables)
    {
        switch (plc)
        {
            case S7Plc s7plc:
                return ReadS7(s7plc, variables.Cast<S7VariableViewModel>());
        }

        foreach (var variable in variables)
        {
            variable.PushValue(new ReceivedValue(new Random().Next(3), DateTimeOffset.Now));
        }

        return Task.CompletedTask;
    }

    private async Task ReadS7(S7Plc plc, IEnumerable<S7VariableViewModel> variables)
    {
        object GetValue(IDataItem dataItem)
        {
            return dataItem.GetType().GetProperty(nameof(IDataItem<int>.Value))!.GetValue(dataItem)!;
        }

        var pairs = variables.Select(v => (Variable: v, DataItem: DataItemBuilder.BuildDataItem(v.Address, v.Length))).ToList();

        var conn = plc.CreateConnection();
        await conn.Open();
        try
        {
            await conn.Read(pairs.Select(p => p.DataItem));
        }
        finally
        {
            conn.Close();
        }

        foreach (var (variable, dataItem) in pairs)
        {
            variable.PushValue(new ReceivedValue(GetValue(dataItem), DateTimeOffset.Now));
        }
    }
}