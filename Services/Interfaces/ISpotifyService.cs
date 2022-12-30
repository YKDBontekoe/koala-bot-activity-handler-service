using Koala.ActivityHandlerService.Models.Outgoing;

namespace Koala.ActivityHandlerService.Services.Interfaces;

public interface ISpotifyService
{
    Task<SpotifyInfoOutgoing> GetSpotifyInfoAsync(string trackId);
}