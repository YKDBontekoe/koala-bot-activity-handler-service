namespace Koala.ActivityHandlerService.Models.Outgoing;

public class ActivityOutgoing
{
    public string Type { get; set; } = "Activity";
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;

    public SpotifyInfoOutgoing? SpotifyInfo { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    
    public User User { get; set; }
}