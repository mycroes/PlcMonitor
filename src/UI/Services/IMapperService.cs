using System.Collections.Generic;
using PlcMonitor.UI.Models.Storage;
using PlcMonitor.UI.ViewModels.Explorer;

namespace PlcMonitor.UI.Services
{
    public interface IMapperService
    {
        Project MapToStorage(IEnumerable<PlcConnectionNode> plcs);
    }
}