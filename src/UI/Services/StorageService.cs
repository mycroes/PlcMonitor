using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PlcMonitor.UI.Models.Storage;

namespace PlcMonitor.UI.Services
{
    public class StorageService : IStorageService
    {
        public async Task Save(Project project, string fileName)
        {
            var file = new FileInfo(fileName);
            using var stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write);

            await JsonSerializer.SerializeAsync(stream, project);
        }

        public async Task<Project> Load(string fileName)
        {
            var file = new FileInfo(fileName);
            using var stream = file.OpenRead();

            return await JsonSerializer.DeserializeAsync<Project>(stream) ??
                throw new Exception($"Failed to deserialize project from {fileName}.");
        }
    }
}