namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IStorageService
    {
        Task<List<string>> GetAllFileNames();
        Task UploadFile(string name, IFormFile file);
        Task UploadTextFile(string name, string text);
        Task UploadFileStream(string name, Stream file);
    }
}
