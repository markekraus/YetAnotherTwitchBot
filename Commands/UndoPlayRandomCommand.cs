using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class UndoPlayRandomCommand : IBotCommand
    {
        public const string PrimaryCommand = "!undoplayrandom";
        public const string CommandDescription = "Posts !wrongsong to chat, undoing the most recent random song from !playrandom.";
        private Regex CommandRegex = new Regex("!undoplayrandom", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string wrongSong = "!wrongsong";
        private ILogger<UndoPlayRandomCommand> _logger;
        public UndoPlayRandomCommand(ILogger<UndoPlayRandomCommand> Logger)
        {
            _logger = Logger;
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
            return wrongSong;
        }

        public bool ShouldRun(string Command)
        {
            return CommandRegex.IsMatch(Command);
        }
    }
}