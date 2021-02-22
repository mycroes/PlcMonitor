using System.IO;
using System.Threading.Tasks;
using PlcMonitor.UI.Models.Storage;

namespace PlcMonitor.UI.Services
{
    public interface IStorageService
    {
        Task<Project> Load(FileInfo file);
        Task Save(Project project, FileInfo file);
    }
}