using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace QR.Application.Helpers
{
    public static class ImageHelper
    {
        public static async Task<byte[]> ResizeImageAsync(IFormFile file, double scaleFactor, int quality)
        {
            {
                using var stream = file.OpenReadStream();
                using var image = await Image.LoadAsync(stream);

                // Get original dimensions
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                // Calculate new dimensions based on the scale factor
                int newWidth = (int)(originalWidth * scaleFactor);
                int newHeight = (int)(originalHeight * scaleFactor);

                // Resize dynamically while maintaining aspect ratio
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(newWidth, newHeight)
                }));

                // Save to memory stream with optional quality setting
                using var outputStream = new MemoryStream();
                await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = quality }); // Adjust quality if needed

                return outputStream.ToArray();
            }
        }
    }
}