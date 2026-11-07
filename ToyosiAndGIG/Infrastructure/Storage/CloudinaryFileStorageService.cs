namespace ToyosiAndGIG.Infrastructure.Storage
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.Extensions.Logging;
    using ToyosiAndGIG.Application.Interfaces;

    public class CloudinaryFileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryFileStorageService> _logger;

        public CloudinaryFileStorageService(Cloudinary cloudinary, ILogger<CloudinaryFileStorageService> logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string folder)
        {
            // PublicId guarantees uniqueness regardless of the original filename
            var publicId = $"{folder}/{Guid.NewGuid()}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                PublicId = publicId,
                Folder = folder,
                Overwrite = false,
                // Cloudinary's own delivery-time optimization on top of our pre-upload compression
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                _logger.LogError("Cloudinary upload failed for {PublicId}: {Error}", publicId, result.Error.Message);
                throw new InvalidOperationException("File upload failed. Please try again.");
            }

            _logger.LogInformation("File uploaded to Cloudinary: {PublicId}, {SizeKb}KB", publicId, result.Bytes / 1024);

            return result.SecureUrl.ToString();
        }

        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl)) return;

            var publicId = ExtractPublicIdFromUrl(fileUrl);
            if (publicId == null)
            {
                _logger.LogWarning("Could not extract Cloudinary public ID from URL: {Url}", fileUrl);
                return;
            }

            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result != "ok" && result.Result != "not found")
                {
                    _logger.LogWarning("Cloudinary deletion returned unexpected result for {PublicId}: {Result}", publicId, result.Result);
                }
                else
                {
                    _logger.LogInformation("File deleted from Cloudinary: {PublicId}", publicId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file from Cloudinary: {PublicId}", publicId);
                // Don't throw — a failed cleanup shouldn't block the calling operation
            }
        }

        // Cloudinary URLs look like:
        // https://res.cloudinary.com/{cloud_name}/image/upload/v1234567890/bookings/flyers/{guid}.jpg
        // The public ID is everything after the version segment, minus the file extension.
        private static string ExtractPublicIdFromUrl(string url)
        {
            try
            {
                var uploadIndex = url.IndexOf("/upload/", StringComparison.OrdinalIgnoreCase);
                if (uploadIndex == -1) return null;

                var afterUpload = url[(uploadIndex + "/upload/".Length)..];

                // Strip the version segment (e.g. "v1234567890/") if present
                var segments = afterUpload.Split('/');
                var startIndex = segments[0].StartsWith("v") && segments[0].Length > 1 && segments[0][1..].All(char.IsDigit) ? 1 : 0;

                var pathWithExtension = string.Join('/', segments[startIndex..]);
                var lastDotIndex = pathWithExtension.LastIndexOf('.');

                return lastDotIndex == -1 ? pathWithExtension : pathWithExtension[..lastDotIndex];
            }
            catch
            {
                return null;
            }
        }
    }
}
