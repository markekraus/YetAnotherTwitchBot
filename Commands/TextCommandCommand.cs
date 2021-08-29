using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class TextCommandCommand : IBotCommand
    {
        public const string PrimaryCommand = "!textcommands";
        public const string CommandDescription = "Command processor for Text Commands.";
        private ILogger<TextCommandCommand> _logger;
        private ITextCommandService _service;
        public TextCommandCommand(
            ILogger<TextCommandCommand> Logger,
            ITextCommandService TextCommandService)
        {
            _logger = Logger;
            _service = TextCommandService;
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
            var command = _service.GetTextCommands().First(command => command.Enabled && command.Command.ToLowerInvariant() == ChatCommand.Command.ToLowerInvariant());
            if(command != null)
            {
                return _service.ParseTemplate(command, ChatMessage, ChatCommand);
            }
            else
            {
                return $"Command '{ChatCommand.Command}' not found.";
            }
        }

        public bool ShouldRun(string Command)
        {
            var commands = _service.GetTextCommands();
            return commands.Any( command => command.Enabled && command.Command.ToLowerInvariant() == Command.ToLowerInvariant());
        }
    }
}