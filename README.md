# Yet Another Twitch Bot

A C#/Blazor Twitch Bot with a web UI for managing settings, joining/leaving channels, and enabling/disabling/commands.

## Supported Commands

* !test - A test command used for testing purposes.
* !catfact - Gives chat a random Cat Fact.
* !isslocation - Gives the current latitude and longitude for the International Space Station.
* !playrandom - Sends a StreamElements song request command to the channel using a random song from Spotify liked songs.
* !undoplayrandom - Posts !wrongsong to chat, undoing the most recent random song from !playrandom.
* !urbandictionary - Provides a Urban Dictionary definition.
* !roll - A dice roll command for D&D style dice.

## How to Use

1. Download the [latest release](https://github.com/markekraus/YetAnotherTwitchBot/releases/latest)
1. Extract the zip file
1. run `YetAnotherTwitchBot.exe`
1. Navigate to [https://localhost:42069/settings](https://localhost:42069/settings)
1. If your browser complains about the certificate security, accept the risks and continue.
1. Configure the settings

NOTE: You may need to allow or unblock the app in your antivirus, firewall, or application protection settings.

## Creating Custom C# Commands

1. Create a class that implements [`IBotCommand`](Interfaces/IBotCommand.cs)
1. Register the command in [`ServicesConfiguration`](Services/ServicesConfiguration.cs)

see [`TestCommand`](Commands/TestCommand.cs) for a basic example command.

## Using the Spotify Features

In order to use the Spotify features, you must create an application in the [Spotify Developer Portal](https://developer.spotify.com/dashboard/login).
You will need to take note of the Client ID and Client Secret to configure in [https://localhost:42069/settings](https://localhost:42069/settings).
You will also need to add the following Redirect URIs:

* `https://localhost:42069/spotifycallback`
* `http://localhost:42068/spotifycallback`
