﻿using Koala.ActivityHandlerService.Options;
using Koala.ActivityHandlerService.Repositories;
using Koala.ActivityHandlerService.Repositories.Interfaces;
using Koala.ActivityHandlerService.Services;
using Koala.ActivityHandlerService.Services.Interfaces;
using Koala.ActivityMusicHandlerService;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;

namespace Koala.ActivityHandlerService;

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
        var config = SpotifyClientConfig.CreateDefault();
        var request = new ClientCredentialsRequest(spotifyOptions.ClientId, spotifyOptions.ClientSecret);

        ClientCredentialsTokenResponse response;
        try
        {
            response = new OAuthClient(config).RequestToken(request).Result;
        } catch (Exception e)
        {
            throw new Exception("Could not get token from Spotify", e);
        }

        var spotifyClient = new SpotifyClient(config.WithToken(response.AccessToken));
        services.AddSingleton(spotifyClient);
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