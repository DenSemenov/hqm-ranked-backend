using hqm_ranked_backend.Models.ViewModels;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IImageGeneratorService
    {
        Image<Rgba32> GenerateImage();
    }
}
