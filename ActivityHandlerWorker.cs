using Koala.ActivityHandlerService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.ActivityHandlerService;

public class ActivityHandlerWorker : IHostedService
{
    private readonly IMessageHandler _messageHandler;

    public ActivityHandlerWorker(IMessageHandler messageHandler)
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