using PlcMonitor.UI.Models.Storage;
using PlcMonitor.UI.ViewModels;

namespace PlcMonitor.UI.Services
{
    public interface IMapperService
    {
        Project MapToStorage(ProjectViewModel project);
        ProjectViewModel MapFromStorage(Project project);
    }
}