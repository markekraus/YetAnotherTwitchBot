using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;
using YetAnotherTwitchBot.Options;

namespace YetAnotherTwitchBot.Services
{
    public class TextCommandService : ITextCommandService
    {
        private ILogger<TextCommandService> _logger;
        private IOptionsMonitor<TextCommandOptions> _options;
        private SettingsHelper _settingsHelper;
        private IEnumerable<IBotCommand> _botCommands;
        public TextCommandService(
            ILogger<TextCommandService> Logger,
            IOptionsMonitor<TextCommandOptions> Options,
            SettingsHelper SettingsHelper,
            IEnumerable<IBotCommand> BotCommands)
        {
            _logger = Logger;
            _options = Options;
            _settingsHelper = SettingsHelper;
            _botCommands = BotCommands;
        }

        public void AddTextCommand(TextCommand Command)
        {
            if(
                _botCommands.Any( command => command.GetCommandName().ToLowerInvariant() == Command.Command.ToLowerInvariant())
                ||
                _options.CurrentValue.Commands.Any( command => command.Command.ToLowerInvariant() == Command.Command.ToLowerInvariant())
            )
            {
                var errorMessage = $"Command '{Command.Command}' already exists.";
                _logger.LogError(errorMessage);
                throw new System.Data.DuplicateNameException(errorMessage);
            }
            _logger.LogInformation($"Adding command '{Command.Command}'...");
            var newCommands = new List<TextCommand>(_options.CurrentValue.Commands);
            newCommands.Add(Command);
            var newOptions = new TextCommandOptions()
            {
                Commands = newCommands
            };
            UpdateSettings(newOptions);
            _logger.LogInformation($"Command '{Command.Command}' added.");
        }

        public IList<TextCommand> GetTextCommands()
        {
            return new List<TextCommand>(_options.CurrentValue.Commands);
        }

        public void RemoveTextCommand(string CommandName)
        {
            RemoveTextCommand(new TextCommand(){Command = CommandName});
        }

        public void RemoveTextCommand(TextCommand Command)
        {
            _logger.LogInformation($"Removing command '{Command.Command}'...");
            var newCommands = new List<TextCommand>(_options.CurrentValue.Commands);
            var found = newCommands.RemoveAll( command => command.Command.ToLowerInvariant() == Command.Command.ToLowerInvariant());
            if (found > 0)
            {
                var newOptions = new TextCommandOptions()
                {
                    Commands = newCommands
                };
                UpdateSettings(newOptions);
                _logger.LogInformation($"Command '{Command.Command}' removed.");
            }
            else
            {
                _logger.LogInformation($"Command '{Command.Command}' was not found and therefore not removed.");
            }
        }

        public void UpdateSettings(TextCommandOptions Options)
        {
            _settingsHelper.AddOrUpdateAppSetting<TextCommandOptions>(TextCommandOptions.Section, Options);
        }
    }
}