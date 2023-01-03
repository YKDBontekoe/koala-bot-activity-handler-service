using Koala.ActivityMusicHandlerService.Options;
using Koala.ActivityMusicHandlerService.Repositories;
using Koala.ActivityMusicHandlerService.Repositories.Interfaces;
using Koala.ActivityMusicHandlerService.Services;
using Koala.ActivityMusicHandlerService.Services.Interfaces;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;

namespace Koala.ActivityMusicHandlerService;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;

                    builder
                        .SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();
                }
            )
            .ConfigureServices(ConfigureDelegate)
            .UseConsoleLifetime()
            .Build();

        await host.RunAsync();
    }

    private static void ConfigureDelegate(HostBuilderContext hostContext, IServiceCollection services)
    {
        ConfigureOptions(services, hostContext.Configuration);
        ConfigureServiceBus(services);
        ConfigureSpotifyClient(services);

        services.AddTransient<ISpotifyRepository, HttpSpotifyRepository>();
        services.AddTransient<ISpotifyService, SpotifyService>();
        services.AddTransient<IMessageHandler, MessageHandler>();
        services.AddHostedService<ActivityMusicHandlerWorker>();
    }

    // Configure options for the application to use in the services
    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.ServiceBus));
        services.Configure<SpotifyOptions>(configuration.GetSection(SpotifyOptions.Spotify));
    }
    
    // Configure the spotify client with the token
    private static void ConfigureSpotifyClient(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var spotifyOptions = serviceProvider.GetService<IOptions<SpotifyOptions>>()?.Value;
        
        ArgumentNullException.ThrowIfNull(spotifyOptions);
        var config = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(new ClientCredentialsAuthenticator(spotifyOptions.ClientId, spotifyOptions.ClientSecret));

        var spotify = new SpotifyClient(config);
        services.AddSingleton(spotify);
    }

    // Configure the Azure Service Bus client with the connection string
    private static void ConfigureServiceBus(IServiceCollection services)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(services.BuildServiceProvider().GetRequiredService<IOptions<ServiceBusOptions>>().Value.ConnectionString);
        });
    }
}