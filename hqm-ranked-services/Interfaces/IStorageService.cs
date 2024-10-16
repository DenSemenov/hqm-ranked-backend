using hqm_ranked_backend.Common;
using Microsoft.AspNetCore.Http;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IStorageService
    {
        Task<List<string>> GetAllFileNames();
        Task<string> GetStorage();
        Task<StorageType> UploadFile(string name, IFormFile file);
        Task<StorageType> UploadTextFile(string name, string text);
        Task<StorageType> UploadFileStream(string name, Stream file);
        Task<string> LoadTextFile(string name);
        Task<string> RemoveFile(string name);
        Task RemoveFiles(DateTime before);
    }
}
