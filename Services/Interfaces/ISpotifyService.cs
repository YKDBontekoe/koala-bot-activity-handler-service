using Koala.ActivityHandlerService.Models.DTOs;

namespace Koala.ActivityHandlerService.Services.Interfaces;

public interface ISpotifyService
{
    Task<TrackInfoDTO> GetSpotifyInfoAsync(string trackId);
}