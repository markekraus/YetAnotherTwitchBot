using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class BrbCommand : IBotCommand
    {
        public const string PrimaryCommand = "!brb";

        private Regex CommandRegex = new Regex("[!]{0,1}brb", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public const string CommandDescription = "A test command used for testing purposes.";
        private ILogger<BrbCommand> _logger;
        public BrbCommand(ILogger<BrbCommand> Logger)
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
            return $"We'll see you soon enough, @{ChatMessage.DisplayName}!";
        }

        public bool ShouldRun(string Command)
        {
            return CommandRegex.IsMatch(Command);
        }
    }
}