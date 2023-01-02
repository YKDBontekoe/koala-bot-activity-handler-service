using Koala.ActivityMusicHandlerService.Models;
using Koala.ActivityMusicHandlerService.Models.Outgoing;
using Koala.ActivityMusicHandlerService.Repositories.Interfaces;
using Koala.ActivityMusicHandlerService.Services.Interfaces;

namespace Koala.ActivityMusicHandlerService.Services;

public class SpotifyService : ISpotifyService
{
    private readonly ISpotifyRepository _spotifyRepository;

    public SpotifyService(ISpotifyRepository spotifyRepository)
    {
        _spotifyRepository = spotifyRepository;
    }

    public async Task<SpotifyInfoOutgoing> GetSpotifyInfoAsync(string trackId)
    {
        var trackDto = await _spotifyRepository.GetTrackById(trackId);

        var track = new Track()
        {
            Album = trackDto.Album,
            Name = trackDto.Name,
            PreviewUrl = trackDto.PreviewUrl,
            Duration = trackDto.Duration,
            Popularity = trackDto.Popularity,
            Explicit = trackDto.Explicit,
            Id = trackDto.Id,
            Uri = trackDto.Uri
        };

        track.Artists = new List<Artist>();
        foreach (var artistId in trackDto.ArtistIds)
        {
            var artist = await _spotifyRepository.GetSeveralArtistsByIds(artistId);
            track.Artists.Add(artist);
        }
        
        var trackAudioFeatures = await _spotifyRepository.GetTrackAudioFeaturesById(trackId);
        
        var spotifyInfo = new SpotifyInfoOutgoing()
        {
            Track = track,
            TrackAudioFeatures = trackAudioFeatures,
        };

        return spotifyInfo;
    }
}