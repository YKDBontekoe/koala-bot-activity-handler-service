using Koala.ActivityMusicHandlerService.Models;
using Koala.ActivityMusicHandlerService.Models.DTOs;
using Koala.ActivityMusicHandlerService.Repositories.Interfaces;
using SpotifyAPI.Web;
using TrackAudioFeatures = Koala.ActivityMusicHandlerService.Models.TrackAudioFeatures;

namespace Koala.ActivityMusicHandlerService.Repositories;

public class HttpSpotifyRepository : ISpotifyRepository
{
    private readonly SpotifyClient _spotifyClient;
    
    public HttpSpotifyRepository(SpotifyClient spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }

    public async Task<TrackDTO> GetTrackById(string id)
    {
        var fullTrack = await _spotifyClient.Tracks.Get(id);
        var track = new TrackDTO
        {
            Id = fullTrack.Id,
            Name = fullTrack.Name,
            Duration = fullTrack.DurationMs,
            Explicit = fullTrack.Explicit,
            Popularity = fullTrack.Popularity,
            PreviewUrl = fullTrack.PreviewUrl,
            Uri = fullTrack.Uri,
            Album = new Album
            {
                Id = fullTrack.Album.Id,
                Name = fullTrack.Album.Name,
                Uri = fullTrack.Album.Uri,
                ReleaseDate = fullTrack.Album.ReleaseDate,
                ReleaseDatePrecision = fullTrack.Album.ReleaseDatePrecision
            },
            ArtistIds = fullTrack.Artists.Select(a => a.Id).ToList(),
        };

        return track;
    }

    public async Task<TrackAudioFeatures> GetTrackAudioFeaturesById(string id)
    {
        var audioFeatures = await _spotifyClient.Tracks.GetAudioFeatures(id);
        var trackAudioFeatures = new TrackAudioFeatures
        {
            Id = audioFeatures.Id,
            Acousticness = audioFeatures.Acousticness,
            Danceability = audioFeatures.Danceability,
            Energy = audioFeatures.Energy,
            Instrumentalness = audioFeatures.Instrumentalness,
            Key = audioFeatures.Key,
            Liveness = audioFeatures.Liveness,
            Loudness = audioFeatures.Loudness,
            Mode = audioFeatures.Mode,
            Speechiness = audioFeatures.Speechiness,
            Tempo = audioFeatures.Tempo,
            TimeSignature = audioFeatures.TimeSignature,
            Valence = audioFeatures.Valence
        };
        
        return trackAudioFeatures;
    }

    public async Task<Artist> GetSeveralArtistsByIds(string id)
    {
        var artist = await _spotifyClient.Artists.Get(id);
        
        var resultArtist = new Artist
        {
            Id = artist.Id,
            Name = artist.Name,
            Uri = artist.Uri,
            Genres = artist.Genres,
            Popularity = artist.Popularity
        };

        return resultArtist;
    }
}