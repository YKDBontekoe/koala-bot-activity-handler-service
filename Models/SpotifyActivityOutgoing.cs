namespace Koala.ActivityHandlerService.Models;

public class SpotifyActivityOutgoing : Activity
{
    public Track Track { get; set; }
    public TrackAudioFeatures TrackAudioFeatures { get; set; }
}