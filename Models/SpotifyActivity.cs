using Koala.ActivityPublisherService.Models;

namespace Koala.ActivityHandlerService.Models;

public class SpotifyActivity : Activity
{
    public required SpotifyInfo SpotifyInfo { get; set; }
}