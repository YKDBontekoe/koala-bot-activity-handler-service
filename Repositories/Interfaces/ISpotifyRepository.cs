using Koala.ActivityHandlerService.Models;
using Koala.ActivityHandlerService.Models.DTOs;

namespace Koala.ActivityHandlerService.Repositories.Interfaces;

public interface ISpotifyRepository
{
    Task<TrackDTO> GetTrackById(string id);
    
    Task<TrackAudioFeatures> GetTrackAudioFeaturesById(string id);

    Task<Artist> GetSeveralArtistsByIds(string id);
}