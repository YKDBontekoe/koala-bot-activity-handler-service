namespace Koala.ActivityHandlerService.Options;

public class ServiceBusOptions
{
    public const string ServiceBus = "ServiceBus";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string UserMusicQueueName { get; set; } = string.Empty;
    public string ConsumerQueueName { get; set; } = string.Empty;
    public string SendMessageQueueName { get; set; } = string.Empty;
}