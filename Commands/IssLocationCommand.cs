using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class IssLocationCommand : IBotCommand
    {
        public const string PrimaryCommand = "!isslocation";
        public const string CommandDescription = "Gives the current latitude and longitude for the International Space Station.";
        private ILogger<IssLocationCommand> _logger;
        private HttpClient _client;
        private Regex CommandRex = new Regex("!iss|[!]{0,1}issloc|[!]{0,1}isslocation|whereisiss|isswhere", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Uri ApiUrl = new Uri("http://api.open-notify.org/iss-now.json");
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public IssLocationCommand(
            ILogger<IssLocationCommand> Logger,
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
            string result = string.Empty;
            string message = string.Empty;
            try
            {
                result = _client.GetStringAsync(ApiUrl).GetAwaiter().GetResult();
                _logger.LogInformation($"Received HTTP message: '{result}'");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "HttpClient Failed");
            }
            try
            {
                var loc = System.Text.Json.JsonSerializer.Deserialize<IssLocation>(result, options);
                message = $"The International Space Station is currently at Longitude: {loc.IssPosition.Longitude}, Latitude: {loc.IssPosition.Latitude}.";
                _logger.LogInformation($"Parsed Fact: '{message}'");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "JSON Deserialization Failed");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Sorry, I was unable to get the ISS's current location.";
                _logger.LogError("Message is null or white space.");
            }
            return message;
            // _logger.LogInformation("Sending request to API");
            // var response = _client.GetAsync(ApiUrl).GetAwaiter().GetResult();
            // _logger.LogInformation("Converting request to object");
            // IssLocation outcome = response.Content.ReadAsAsync<IssLocation>().GetAwaiter().GetResult();
            // _logger.LogInformation($"Returning results Longitude: {outcome?.IssPosition?.Longitude}, Latitude: {outcome?.IssPosition?.Latitude}");
            // return $"The international Space Station is currently at Longitude: {outcome.IssPosition.Longitude}, Latitude: {outcome.IssPosition.Latitude}.";
        }

        public bool ShouldRun(string Command)
        {
            return CommandRex.IsMatch(Command);
        }
    }
}