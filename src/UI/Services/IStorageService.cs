using System.Threading.Tasks;
using PlcMonitor.UI.Models.Storage;

namespace PlcMonitor.UI.Services
{
    public interface IStorageService
    {
        Task<Project> Load(string fileName);
        Task Save(Project project, string fileName);
    }
}