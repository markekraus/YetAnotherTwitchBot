using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class CatFactCommand : IBotCommand
    {
        public const string PrimaryCommand = "!catfact";
        public const string CommandDescription = "Gives chat a random Cat Fact.";
        private ILogger<CatFactCommand> _logger;
        private HttpClient _client;
        private Regex CommandRex = new Regex("[!]{0,1}catfact[s]{0,1}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Uri ApiUrl = new Uri("https://catfact.ninja/fact");
        public CatFactCommand(
            ILogger<CatFactCommand> Logger,
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
            var response = _client.GetAsync(ApiUrl).GetAwaiter().GetResult();
            var fact = response.Content.ReadAsAsync<CatFact>().GetAwaiter().GetResult();
            return fact.Fact;
        }

        public bool ShouldRun(string Command)
        {
            return CommandRex.IsMatch(Command);
        }
    }
}