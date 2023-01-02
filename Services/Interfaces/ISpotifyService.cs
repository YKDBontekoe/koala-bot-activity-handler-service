using Koala.ActivityMusicHandlerService.Models.Outgoing;

namespace Koala.ActivityMusicHandlerService.Services.Interfaces;

public interface ISpotifyService
{
    Task<SpotifyInfoOutgoing> GetSpotifyInfoAsync(string trackId);
}