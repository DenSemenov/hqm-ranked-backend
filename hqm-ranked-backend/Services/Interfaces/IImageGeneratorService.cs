using hqm_ranked_backend.Models.ViewModels;
using System.Drawing;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IImageGeneratorService
    {
        Bitmap GenerateImage();
    }
}
