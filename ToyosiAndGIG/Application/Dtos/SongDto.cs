namespace ToyosiAndGIG.Application.Dtos
{
    public class SongDto
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsDownloadable { get; set; } = false;
        public IFormFile FileUrl { get; set; }
        public IFormFile CoverUrl { get; set; }
        public string YoutubeLink { get; set; }
        public string AudioMackLink { get; set; }
        public string SpotifyMackLink { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string UploadedBy { get; set; }
        public DateTime? LastUpdateAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
