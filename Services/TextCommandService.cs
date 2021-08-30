using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;
using YetAnotherTwitchBot.Options;
using YetAnotherTwitchBot.Statics;

namespace YetAnotherTwitchBot.Services
{
    public class TextCommandService : ITextCommandService
    {
        private ILogger<TextCommandService> _logger;
        private IOptionsMonitor<TextCommandOptions> _options;
        private ISettingsHelper _settingsHelper;
        public TextCommandService(
            ILogger<TextCommandService> Logger,
            IOptionsMonitor<TextCommandOptions> Options,
            ISettingsHelper SettingsHelper)
        {
            _logger = Logger;
            _options = Options;
            _settingsHelper = SettingsHelper;
        }

        public void AddTextCommand(TextCommand Command)
        {
            if(_options.CurrentValue.Commands.Any( command => command.Command.ToLowerInvariant() == Command.Command.ToLowerInvariant()))
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

        public string ParseTemplate(TextCommand TextCommand, ChatMessage ChatMessage, TwitchChatCommand ChatCommand)
        {
            var response = TextCommand.Template;
            var parseDictionary = new Dictionary<string, string>()
            {
                {Regex.Escape(TextCommandVariables.Channel),ChatMessage.Channel},
                {Regex.Escape(TextCommandVariables.BotUsername),ChatMessage.BotUsername},
                {Regex.Escape(TextCommandVariables.DisplayName),ChatMessage.DisplayName},
                {Regex.Escape(TextCommandVariables.SubscribedMonthCount),ChatMessage.SubscribedMonthCount.ToString()},
                {Regex.Escape(TextCommandVariables.Username),ChatMessage.Username},
                {Regex.Escape(TextCommandVariables.Command),ChatCommand.Command}
            };
            if(ChatCommand.HasParameters)
            {
                for (int i = 0; i < ChatCommand.Parameters.Count; i++)
                {
                    parseDictionary.Add(Regex.Escape($"{{agr{i}}}"), ChatCommand.Parameters[i]);
                }
            }
            foreach (var entry in parseDictionary)
            {
                response = Regex.Replace(response, entry.Key, entry.Value, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            }
            return response;
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