using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class TestCommand : IBotCommand
    {
        public const string PrimaryCommand = "!test";
        public const string CommandDescription = "A test command used for testing purposes.";
        private Regex CommandRegex = new Regex("!test", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private ILogger<TestCommand> _logger;
        public TestCommand(ILogger<TestCommand> Logger)
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
            return "Testing 123";
        }

        public bool ShouldRun(string Command)
        {
            return CommandRegex.IsMatch(Command);
        }
    }
}