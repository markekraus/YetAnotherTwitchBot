using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;
using YetAnotherTwitchBot.Services;

namespace YetAnotherTwitchBot.Commands
{
    public class PlayRandomCommand : IBotCommand
    {
        public const string PrimaryCommand = "!playrandom";
        public const string CommandDescription = "Sends a song request command to the channel using a random song from Spotify liked songs.";
        private ILogger<PlayRandomCommand> _logger;
        private Regex CommandRegex = new Regex("!playrandom", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private SpotifyHandler _spotifyHandler;
        public PlayRandomCommand(
            ILogger<PlayRandomCommand> Logger,
            SpotifyHandler SpotifyHandler)
        {
            _logger = Logger;
            _spotifyHandler = SpotifyHandler;
        }

        public string GetCommandDescription()
        {
            return CommandDescription;
        }

        public string GetCommandName()
        {
            return PrimaryCommand;
        }

        public string Run(ChatMessage ChatMessage, TwitchChatCommand ChatCommand)
        {
            return _spotifyHandler.GetRandomTrack();
        }

        public bool ShouldRun(string Command)
        {
            return CommandRegex.IsMatch(Command);
        }
    }
}