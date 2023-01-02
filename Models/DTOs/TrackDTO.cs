namespace Koala.ActivityMusicHandlerService.Models.DTOs;

public class TrackDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Duration { get; set; }
    public bool Explicit { get; set; }
    public int Popularity { get; set; }
    public string PreviewUrl { get; set; }
    public string Uri { get; set; }
    public List<string> ArtistIds { get; set; }
    public Album Album { get; set; }
}