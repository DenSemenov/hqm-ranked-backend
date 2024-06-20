using hqm_ranked_backend.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace hqm_ranked_backend.Services
{
    public class ImageGeneratorService:IImageGeneratorService
    {

        public Image<Rgba32> GenerateImage()
        {
            var w = 100;
            var h = 100;
            var image = new Image<Rgba32>(w, h);

            int grain = 5;
            Random random = new Random();
            double blockout = random.NextDouble();

            int[] colorArray = new int[3];
            image.Mutate(g => g.BackgroundColor(Color.Transparent));

            Action<int, int> RandomRGB = (min, max) =>
            {
                for (int i = 0; i < 3; i++)
                {
                    colorArray[i] = random.Next(min, max);
                }
            };

            Action PokeOut = () =>
            {
                RandomRGB(0, 255);
                int posX = 0;
                int posY = 0;
                int colorRange = 5;
                var startFillRed = colorArray[0];
                var startFillGreen = colorArray[1];
                var startFillBlue = colorArray[2];
                var brush = Color.FromRgb((byte)startFillRed, (byte)startFillGreen, (byte)startFillBlue);

                for (int y = 0; y < grain; y++)
                {
                    for (int x = 0; x < grain; x++)
                    {
                        if (blockout < 0.4)
                        {
                            var x1 = posX;
                            var y1 = posY;
                            var w1 = w / grain;
                            var h1 = h / grain;

                            for (int i = x1; i < x1 + w1; i++) {
                                for (int j = y1; j < y1 + h1; j++)
                                {
                                    image[i, j] = brush;
                                }
                            }

                            var x2 = w - posX - w / grain;
                            var y2 = posY;
                            var w2 = w / grain;
                            var h2 = h / grain;

                            for (int i = x2; i < x2 + w2; i++)
                            {
                                for (int j = y2; j < y2 + h2; j++)
                                {
                                    image[i, j] = brush;
                                }
                            }

                            posX += w / grain;
                        }
                        else
                        {
                            startFillRed -= colorRange;
                            startFillGreen += colorRange;
                            startFillBlue += colorRange;
                            brush = Color.FromRgb((byte)Math.Clamp(startFillRed, 0, 255), (byte)Math.Clamp(startFillGreen, 0, 255), (byte)Math.Clamp(startFillBlue, 0, 255));
                            posX += w / grain;
                        }
                        blockout = random.NextDouble();
                    }
                    posY += h / grain;
                    posX = 0;
                }
            };

            PokeOut();

            return image;
        }
    }
}
