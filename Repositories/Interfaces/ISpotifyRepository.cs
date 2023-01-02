using Koala.ActivityMusicHandlerService.Models;
using Koala.ActivityMusicHandlerService.Models.DTOs;

namespace Koala.ActivityMusicHandlerService.Repositories.Interfaces;

public interface ISpotifyRepository
{
    Task<TrackDTO> GetTrackById(string id);
    
    Task<TrackAudioFeatures> GetTrackAudioFeaturesById(string id);

    Task<Artist> GetSeveralArtistsByIds(string id);
}