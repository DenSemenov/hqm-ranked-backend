namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IStorageService
    {
        Task UploadFile(string name, IFormFile file);
        Task UploadTextFile(string name, string text);
    }
}
