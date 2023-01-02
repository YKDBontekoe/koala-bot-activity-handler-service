namespace Koala.ActivityMusicHandlerService.Services.Interfaces;

public interface IMessageHandler
{
    Task InitializeAsync();
    Task CloseQueueAsync();
    Task DisposeAsync();
}