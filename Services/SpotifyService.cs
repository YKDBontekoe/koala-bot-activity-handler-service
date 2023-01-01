using Koala.ActivityHandlerService.Models;
using Koala.ActivityHandlerService.Models.DTOs;
using Koala.ActivityHandlerService.Repositories.Interfaces;
using Koala.ActivityHandlerService.Services.Interfaces;

namespace Koala.ActivityHandlerService.Services;

public class SpotifyService : ISpotifyService
{
    private readonly ISpotifyRepository _spotifyRepository;

    public SpotifyService(ISpotifyRepository spotifyRepository)
    {
        _spotifyRepository = spotifyRepository;
    }

    public async Task<TrackInfoDTO> GetSpotifyInfoAsync(string trackId)
    {
        var trackDto = await _spotifyRepository.GetTrackById(trackId);
        var trackAudioFeatures = await _spotifyRepository.GetTrackAudioFeaturesById(trackId);
        var track = MapTrackDtoToTrack(trackDto);
        var artists = await GetArtistsByIds(trackDto.ArtistIds);

        track.Artists = artists;

        var spotifyInfo = new TrackInfoDTO
        {
            Track = track,
            TrackAudioFeatures = trackAudioFeatures,
        };

        return spotifyInfo;
    }

    private async Task<List<Artist>> GetArtistsByIds(IEnumerable<string> artistIds)
    {
        var artists = new List<Artist>();
        foreach (var artistId in artistIds)
        {
            var artist = await _spotifyRepository.GetSeveralArtistsByIds(artistId);
            artists.Add(artist);
        }

        return artists;
    }

    private static Track MapTrackDtoToTrack(TrackDTO trackDto)
    {
        return new Track
        {
            Album = trackDto.Album,
            Name = trackDto.Name,
            PreviewUrl = trackDto.PreviewUrl,
            Duration = trackDto.Duration,
            Popularity = trackDto.Popularity,
            Explicit = trackDto.Explicit,
            Id = trackDto.Id,
            Uri = trackDto.Uri,
        };
    }
}