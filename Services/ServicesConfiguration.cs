using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Models;
using YetAnotherTwitchBot.Commands;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Options;
using YetAnotherTwitchBot.Services;

public static class ServicesConfiguration
{
    public static void AddBotServices(
        this IServiceCollection services,
        IConfiguration Configuration)
    {
        // Twitch Bot Services
        services.Configure<ClientOptions>(_options => {
                _options.MessagesAllowedInPeriod = 750;
                _options.ThrottlingPeriod = TimeSpan.FromSeconds(30);
                _options.ReconnectionPolicy = new ReconnectionPolicy(3000,3000,100);
            });
        services.Configure<TwitchOptions>(Configuration.GetSection(TwitchOptions.Section));
        services.Configure<CommandManagementOptions>(Configuration.GetSection(CommandManagementOptions.Section));
        services.AddSingleton<ISettingsHelper, SettingsHelper>();
        services.AddTransient<ITwitchClient, TwitchClient>();
        services.AddSingleton<IBotCommandService, BotCommandService>();

        // Dev Helper
        services.AddSingleton<DevHelperService>();

        // Test Command
        services.AddSingleton<IBotCommand, TestCommand>();

        // Cat Fact
        services.AddHttpClient<CatFactCommand>();
        services.AddSingleton<IBotCommand, CatFactCommand>();

        // IIS Location
        services.AddHttpClient<IssLocationCommand>();
        services.AddSingleton<IBotCommand, IssLocationCommand>();

        // Spotify
        services.Configure<SpotifyOptions>(Configuration.GetSection(SpotifyOptions.Section));
        services.AddSingleton<SpotifyHandler>();
        services.AddSingleton<IBotCommand, PlayRandomCommand>();
        services.AddSingleton<IBotCommand, UndoPlayRandomCommand>();

        // Urban Dictionary
        services.AddHttpClient<UrbanDictionaryCommand>();
        services.AddSingleton<IBotCommand, UrbanDictionaryCommand>();

        // Roll command
        services.AddSingleton<IBotCommand, RollCommand>();

        // Text Commands
        services.Configure<TextCommandOptions>(Configuration.GetSection(TextCommandOptions.Section));
        services.AddSingleton<ITextCommandService, TextCommandService>();
        services.AddSingleton<IBotCommand, TextCommandCommand>();

    }
}