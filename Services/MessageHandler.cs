using Azure.Messaging.ServiceBus;
using Koala.ActivityHandlerService.Options;
using Koala.ActivityHandlerService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.ActivityHandlerService.Services;

public class MessageHandler : IMessageHandler
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusOptions _serviceBusOptions;
    private ServiceBusProcessor? _processor;

    public MessageHandler(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions)
    {
        _serviceBusClient = serviceBusClient;
        _serviceBusOptions = serviceBusOptions != null ? serviceBusOptions.Value : throw new ArgumentNullException(nameof(serviceBusOptions));
    }

    public async Task InitializeAsync()
    {
        _processor = _serviceBusClient.CreateProcessor(_serviceBusOptions.UserActivitiesQueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(15),
            PrefetchCount = 100,
        });
        
        try
        {
            // add handler to process messages
            _processor.ProcessMessageAsync += MessagesHandler;

            // add handler to process any errors
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task CloseQueueAsync()
    {
        if (_processor != null) await _processor.CloseAsync();
    }

    public async Task DisposeAsync()
    {
        if (_processor != null) await _processor.DisposeAsync();
    }

    private async Task MessagesHandler(ProcessMessageEventArgs args)
    {
        // Process the message.
        var body = args.Message.Body.ToString();
        
        // Implement logic to process the activity

        var sender = _serviceBusClient.CreateSender(_serviceBusOptions.ConsumerQueueName);
        await sender.SendMessageAsync(new ServiceBusMessage(body));
    }
    
    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // Process the error.
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}