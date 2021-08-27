using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Options;

namespace YetAnotherTwitchBot.Services
{
    public class DevHelperService
    {
        private ILogger<DevHelperService> _logger;
        private IConfiguration configuration;
        private IEnumerable<IBotCommand> _botCommands;
        private IOptionsMonitor<TwitchOptions> _twitchOptions;
        public DevHelperService(
            ILogger<DevHelperService> Logger,
            IConfiguration Configuration,
            IEnumerable<IBotCommand> BotCommands,
            IOptionsMonitor<TwitchOptions> TwitchOptions)
        {
            _logger = Logger;
            configuration = Configuration;
            _botCommands = BotCommands;
            _twitchOptions = TwitchOptions;
        }

        public string GetConfigValue(string Key)
        {
            return configuration.GetValue<string>(Key);
        }

        public int GetCommandCount()
        {
            int count = 0;
            foreach (var command in _botCommands)
            {
                count++;
            }
            return count;
        }

        public IOptionsMonitor<TwitchOptions> GetCurrentTwitchOptionValues()
        {
            return _twitchOptions;
        }
    }
}