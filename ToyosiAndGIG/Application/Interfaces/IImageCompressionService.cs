namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IImageCompressionService
    {
        Task<(Stream Stream, string ContentType, string FileExtension)> CompressAsync(Stream inputStream, string originalFileName);
    }
}
