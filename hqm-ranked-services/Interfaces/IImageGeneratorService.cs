using hqm_ranked_backend.Models.ViewModels;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using hqm_ranked_models.DTO;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IImageGeneratorService
    {
        Image<Rgba32> GenerateImage();
        Image GenerateMatches(List<TourneyMatchesDTO> matches, string tourneyName, string roundName);
    }
}
