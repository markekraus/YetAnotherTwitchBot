using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class UrbanDictionaryCommand : IBotCommand
    {
        public const string PrimaryCommand = "!urbandictionary";
        private static string ApiUrlTemplate = "https://api.urbandictionary.com/v0/define?term={0}";
        public const string CommandDescription = "Provides a Urban Dictionary definition.";
        private ILogger<UrbanDictionaryCommand> _logger;
        private HttpClient _client;
        private Regex CommandRex = new Regex("!urdic|!urban|!urbandict|!urbandictionary!define", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public UrbanDictionaryCommand(
            ILogger<UrbanDictionaryCommand> Logger,
            HttpClient Client)
        {
            _logger = Logger;
            _client = Client;
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
            string message = $"@{ChatMessage.DisplayName} usage: '{PrimaryCommand} <word>' example: '{PrimaryCommand} face' or '{PrimaryCommand} \"double negative\"'";;
            if(ChatCommand.HasParameters)
            {
                var ApiUrl = new Uri(string.Format(ApiUrlTemplate,ChatCommand.Parameters[0]));
                var response = _client.GetAsync(ApiUrl).GetAwaiter().GetResult();
                var definition = response.Content.ReadAsAsync<UrbanDictionaryResponse>().GetAwaiter().GetResult();
                if(!string.IsNullOrWhiteSpace(definition.List[0].definition))
                {
                            message = Regex.Replace(definition.List[0].definition, @"\r\n?|\n|\r", " ");
                            if(message.Length > 400)
                            {
                                message = message.Substring(0,400) + "...";
                            }
                            message = $"{ChatCommand.Parameters[0]} - {message}";
                            _logger.LogInformation($"Parsed Dictionary Entry: '{message}'");
                }
                else
                {
                    message = $"Sorry @{ChatMessage.DisplayName}, I was unable to get an Urban Dictionary Definition.";
                }
            }
            return message;
        }

        public bool ShouldRun(string Command)
        {
            return CommandRex.IsMatch(Command);
        }
    }
}