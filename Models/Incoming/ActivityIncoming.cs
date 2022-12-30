namespace Koala.ActivityHandlerService.Models.Incoming;

public class ActivityIncoming
{
    public string Type { get; set; } = "Activity";
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;

    public SpotifyInfoIncoming? SpotifyInfo { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    
    public User User { get; set; }
}