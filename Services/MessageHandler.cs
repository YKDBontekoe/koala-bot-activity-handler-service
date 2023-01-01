using Azure.Messaging.ServiceBus;
using Koala.ActivityHandlerService.Models;
using Koala.ActivityHandlerService.Models.DTOs;
using Koala.ActivityHandlerService.Options;
using Koala.ActivityHandlerService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.ActivityHandlerService.Services;

public class MessageHandler : IMessageHandler
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusOptions _serviceBusOptions;
    private readonly ISpotifyService _spotifyService;
    private ServiceBusProcessor? _processor;

    public MessageHandler(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions,
        ISpotifyService spotifyService)
    {
        _serviceBusClient = serviceBusClient;
        _spotifyService = spotifyService;
        _serviceBusOptions = serviceBusOptions != null
            ? serviceBusOptions.Value
            : throw new ArgumentNullException(nameof(serviceBusOptions));
    }

    public async Task InitializeAsync()
    {
        _processor = CreateServiceBusProcessor();
        AddMessageHandlers();

        try
        {
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

    private ServiceBusProcessor CreateServiceBusProcessor()
    {
        return _serviceBusClient.CreateProcessor(_serviceBusOptions.UserMusicQueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(15),
            PrefetchCount = 100,
        });
    }

    private void AddMessageHandlers()
    {
        _processor.ProcessMessageAsync += ProcessMessagesAsync;
        _processor.ProcessErrorAsync += ProcessErrorsAsync;
    }

    private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
    {
        var activity = DeserializeActivity(args.Message.Body);
        var spotifyOutgoing = await GetSpotifyInfoAsync(activity);
        var activityOutgoing = CreateActivityOutgoing(activity, spotifyOutgoing);
        await SendMessageAsync(activityOutgoing);
    }

    private static SpotifyActivity DeserializeActivity(ReadOnlyMemory<byte> body)
    {
        return JsonConvert.DeserializeObject<SpotifyActivity>(body.ToString());
    }

    private async Task<TrackInfoDTO> GetSpotifyInfoAsync(SpotifyActivity activity)
    {
        return await _spotifyService.GetSpotifyInfoAsync(activity.SpotifyInfo.TrackId);
    }

    private static SpotifyActivityOutgoing CreateActivityOutgoing(SpotifyActivity activity,
        TrackInfoDTO spotifyOutgoing)
    {
        return new SpotifyActivityOutgoing
        {
            StartedAt = activity.StartedAt,
            Type = activity.Type,
            User = activity.User,
            Track = spotifyOutgoing.Track,
            TrackAudioFeatures = spotifyOutgoing.TrackAudioFeatures,
            Name = activity.Name
        };
    }

    private async Task SendMessageAsync(SpotifyActivityOutgoing activityOutgoing)
    {
        var sender = _serviceBusClient.CreateSender(_serviceBusOptions.ConsumerQueueName);
        await sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(activityOutgoing)));
    }

    private static Task ProcessErrorsAsync(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}