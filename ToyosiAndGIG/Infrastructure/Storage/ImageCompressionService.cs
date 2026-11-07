using ToyosiAndGIG.Application.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace ToyosiAndGIG.Infrastructure.Storage
{
    public class ImageCompressionService : IImageCompressionService
    {
        private const int MaxDimension = 1600;
        private const int JpegQuality = 75;

        private readonly ILogger<ImageCompressionService> _logger;

        public ImageCompressionService(ILogger<ImageCompressionService> logger)
        {
            _logger = logger;
        }

        public async Task<(Stream Stream, string ContentType, string FileExtension)> CompressAsync(Stream inputStream, string originalFileName)
        {
            try
            {
                using var image = await Image.LoadAsync(inputStream);

                if (image.Width > MaxDimension || image.Height > MaxDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxDimension, MaxDimension)
                    }));
                }

                var outputStream = new MemoryStream();
                var encoder = new JpegEncoder { Quality = JpegQuality };
                await image.SaveAsJpegAsync(outputStream, encoder);
                outputStream.Position = 0;

                _logger.LogInformation("Compressed image {FileName}: {Width}x{Height}, {SizeKb}KB",
                    originalFileName, image.Width, image.Height, outputStream.Length / 1024);

                return (outputStream, "image/jpeg", ".jpg");
            }
            catch (UnknownImageFormatException ex)
            {
                _logger.LogError(ex, "File {FileName} is not a valid image format", originalFileName);
                throw new InvalidOperationException("The uploaded file is not a valid image.");
            }
        }
    }
}
