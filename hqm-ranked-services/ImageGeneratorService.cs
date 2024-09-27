using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_models.DTO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Net;

namespace hqm_ranked_backend.Services
{
    public class ImageGeneratorService: IImageGeneratorService
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

        public Image GenerateMatches(List<TourneyMatchesDTO> matches, string tourneyName, string roundName)
        {
            var rowSize = 64;
            var rowWidth = 650;
            var fontSize = 16;
            var imageSize = 48;
            var padding = 16;
            var gap = 8;
            var textTopOffset = imageSize / 2;
            var titleOffset = 48;

            var height = matches.Count * rowSize + padding + titleOffset;

            var image = new Image<Rgba32>(rowWidth, height);
            image.Mutate(g => g.BackgroundColor(Color.Black));

            FontCollection collection = new();
            collection.Add("Azonix.ttf");

            collection.TryGet("Azonix", out FontFamily family);
            var font = family.CreateFont(fontSize, FontStyle.Regular);

            var rowIndex = 0;

            image.Mutate(x => x.DrawText(
                new RichTextOptions(font)
                {
                    Origin = new PointF(padding, padding + titleOffset / 2 - 8),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                tourneyName,
                new Color(Rgba32.ParseHex("#FFFFFFEE"))
            ));

            image.Mutate(x => x.DrawText(
               new RichTextOptions(font)
               {
                   Origin = new PointF(rowWidth - padding, padding + titleOffset / 2 - 8),
                   HorizontalAlignment = HorizontalAlignment.Right,
                   VerticalAlignment = VerticalAlignment.Center,
               },
               roundName,
               new Color(Rgba32.ParseHex("#FFFFFFEE"))
           ));

            var pen = Pens.Solid(new Color(Rgba32.ParseHex("#fdfdfd1f")), 1);
            image.Mutate(x => x.DrawLine(
                pen,
            [new Point(padding, padding + titleOffset - 8), new Point(rowWidth - padding, padding + titleOffset - 8)]
                ));

            foreach (var match in matches)
            {
                var topOffset = rowIndex * rowSize;
                var leftAlign = new RichTextOptions(font)
                {
                    Origin = new PointF(padding + imageSize + gap, topOffset + padding + textTopOffset + titleOffset),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                image.Mutate(x => x.DrawText(
                    leftAlign,
                    match.RedName,
                    new Color(Rgba32.ParseHex("#FFFFFFEE"))
                ));

                var rightAlign = new RichTextOptions(font)
                {
                    Origin = new PointF(rowWidth - padding - imageSize - gap, topOffset + padding + textTopOffset + titleOffset),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                image.Mutate(x => x.DrawText(
                  rightAlign,
                  match.BlueName,
                  new Color(Rgba32.ParseHex("#FFFFFFEE"))
               ));

                var centerAlign = new RichTextOptions(font)
                {
                    Origin = new PointF(rowWidth / 2, topOffset + padding + textTopOffset + titleOffset),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                image.Mutate(x => x.DrawText(
                  centerAlign,
                  "vs",
                  new Color(Rgba32.ParseHex("#FFFFFFEE"))
               ));

                var redImage = Image.Load(ImageUrlToStream(match.RedUrl));
                var blueImage = Image.Load(ImageUrlToStream(match.BlueUrl));

                redImage.Mutate(x => x.Resize(imageSize, imageSize));
                blueImage.Mutate(x => x.Resize(imageSize, imageSize));

                image.Mutate(x => x.DrawImage(
                        redImage,
                        new Point(padding, topOffset + padding + titleOffset),
                        1
                    ));

                image.Mutate(x => x.DrawImage(
                     blueImage,
                    new Point(rowWidth - padding - imageSize, topOffset + padding + titleOffset),
                      1
                    ));

                rowIndex += 1;
            }

            return image;
        }

        public Stream ImageUrlToStream(string imageUrl)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(imageUrl);

                MemoryStream ms = new MemoryStream(imageBytes);

                return ms;
            }
        }
    }
}
