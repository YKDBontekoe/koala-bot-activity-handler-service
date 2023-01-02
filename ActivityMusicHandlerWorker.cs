using Koala.ActivityMusicHandlerService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.ActivityMusicHandlerService;

public class ActivityMusicHandlerWorker : IHostedService
{
    private readonly IMessageHandler _messageHandler;

    public ActivityMusicHandlerWorker(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageHandler.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _messageHandler.DisposeAsync()!;
        await _messageHandler.CloseQueueAsync()!;
    }
}