using hqm_ranked_backend.Services.Interfaces;
using System.Drawing;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace hqm_ranked_backend.Services
{
    public class ImageGeneratorService:IImageGeneratorService
    {
        public Bitmap GenerateImage()
        {
            var w = 100;
            var h = 100;
            var image = new Bitmap(w, h);

            int grain = 5;
            Random random = new Random();
            double blockout = random.NextDouble();

            int[] colorArray = new int[3];

            using (var graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.Transparent);

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
                    SolidBrush brush = new SolidBrush(Color.FromArgb(1,startFillRed, startFillGreen, startFillBlue));

                    for (int y = 0; y < grain; y++)
                    {
                        for (int x = 0; x < grain; x++)
                        {
                            if (blockout < 0.4)
                            {
                                graphics.FillRectangle(brush, posX, posY, w / grain, h / grain);
                                graphics.FillRectangle(brush, w - posX - w / grain, posY, w / grain, h / grain);
                                posX += w / grain;
                            }
                            else
                            {
                                startFillRed -= colorRange;
                                startFillGreen += colorRange;
                                startFillBlue += colorRange;
                                try
                                {
                                    brush = new SolidBrush(Color.FromArgb(1, Math.Clamp(startFillRed, 0, 255), Math.Clamp(startFillGreen, 0, 255), Math.Clamp(startFillBlue, 0, 255)));
                                }
                                catch (Exception ex)
                                {

                                }
                                posX += w / grain;
                            }
                            blockout = random.NextDouble();
                        }
                        posY += h / grain;
                        posX = 0;
                    }
                };

                PokeOut();
            }

            return image;
        }
    }
}
