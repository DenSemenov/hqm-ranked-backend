namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IStorageService
    {
        Task<List<string>> GetAllFileNames();
        Task<string> GetStorage();
        Task<bool> UploadFile(string name, IFormFile file);
        Task<bool> UploadTextFile(string name, string text);
        Task<bool> UploadFileStream(string name, Stream file);
        Task<string> LoadTextFile(string name);
    }
}
