using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;
using YetAnotherTwitchBot.Options;

namespace YetAnotherTwitchBot.Services
{
    public class BotCommandService : IBotCommandService
    {
        private ILogger<BotCommandService> _logger;
        private ITwitchClient _client;
        private IConfiguration _config;
        private IEnumerable<IBotCommand> _botCommands;
        private IOptionsMonitor<TwitchOptions> _twitchOptions;
        private IServiceProvider _services;
        private object _lock = new object();
        private Dictionary<string, IBotCommand> _commandDictionary = new Dictionary<string, IBotCommand>(StringComparer.InvariantCultureIgnoreCase);
        private IOptionsMonitor<CommandManagementOptions> _commandManagementOptions;
        private List<CommandManagementItem> _commandManagementItems = new List<CommandManagementItem>();
        private Timer _timeoutTimer;
        public BotCommandService(
            ILogger<BotCommandService> Logger,
            ITwitchClient TwitchClient,
            IConfiguration Configuration,
            IEnumerable<IBotCommand> BotCommands,
            IOptionsMonitor<TwitchOptions> TwitchOptions,
            IOptionsMonitor<CommandManagementOptions> CommandManagementOptions,
            IServiceProvider Services
        )
        {
            _logger = Logger;
            _client = TwitchClient;
            _config = Configuration;
            _botCommands = BotCommands;
            _twitchOptions = TwitchOptions;
            _commandManagementOptions = CommandManagementOptions;
            _services = Services;

            _timeoutTimer = new Timer()
            {
                AutoReset = false,
                Interval = 600000
            };
            _timeoutTimer.Elapsed += OnPingTimeout;

            foreach (var command in _botCommands)
            {
                if(!_commandDictionary.TryAdd(command.GetCommandName().ToLower(), command))
                {
                    string errorMessage = $"Duplicate command found {command.GetCommandName()}";
                    _logger.LogError(errorMessage);
                    throw new System.Data.DuplicateNameException(errorMessage);
                }
            }
            UpdateCommandOptions();
            _commandManagementOptions.OnChange(UpdateCommandOptions);
            RegisterCallbacks();
            _twitchOptions.OnChange(SettingsUpdate);
            StartIrc();
        }

        private void OnPingTimeout(object sender, ElapsedEventArgs e)
        {
            _logger.LogError("Ping timeout exceeded. Reconnecting...");
            lock (_lock)
            {
                StopIrc();
                _client = _services.GetService<ITwitchClient>();
                RegisterCallbacks();
                StartIrc();
            }
        }

        private void UpdateCommandOptions(CommandManagementOptions Options)
        {
            UpdateCommandOptions();
        }
        private void UpdateCommandOptions()
        {
            _commandManagementItems = new List<CommandManagementItem>();
            foreach (var commandOption in _commandManagementOptions.CurrentValue.CommandSettings)
            {
                _logger.LogInformation($"Processing command options for '{commandOption.CommandName}'");
                if(_commandDictionary.TryGetValue(commandOption.CommandName, out IBotCommand command))
                {
                    _logger.LogInformation($"Command options '{commandOption.CommandName}' matched to command '{command.GetCommandName()}'");
                    commandOption.SetBotCommand(command);
                    _commandManagementItems.Add(commandOption);
                }
            }
            foreach (var command in _botCommands)
            {
                if (!_commandManagementItems.Any(item => (item.CommandName.ToLowerInvariant() == command.GetCommandName().ToLower())))
                {
                    _logger.LogInformation($"Command '{command.GetCommandName()}' did not have any configure options. Using defaults.");
                    _commandManagementItems.Add(new CommandManagementItem(command));
                }
            }
        }

        public void StartIrc()
        {
            if (!_twitchOptions.CurrentValue.Enabled)
            {
                _logger.LogInformation("Disbled. Skipping start");
                return;
            }
            _logger.LogInformation("Connecting...");
            ConnectionCredentials credentials = new ConnectionCredentials(_twitchOptions.CurrentValue.BotUsername, _twitchOptions.CurrentValue.BotIrcPassword);
            _client.Initialize(credentials, _twitchOptions.CurrentValue.Channels as List<string>);
            _client.Connect();
            _timeoutTimer.Start();
        }

        public void StopIrc()
        {
            _timeoutTimer.Stop();
            if (_client.IsConnected)
            {
                _logger.LogInformation("Disconnecting...");
                _client.Disconnect();
                _logger.LogInformation("Disconnected!");
            }
            else
            {
                _logger.LogInformation("Client not connecting. Skipping disconnect.");
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
            _logger.LogInformation($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void SettingsUpdate(TwitchOptions options)
        {
            lock (_lock)
            {
                _logger.LogInformation("Settings Updated!!!");
                StopIrc();
                _client = _services.GetService<ITwitchClient>();
                RegisterCallbacks();
                StartIrc();
            }
        }

        public IList<IBotCommand> GetEnabledBotCommands()
        {
            return _commandManagementItems.Where(item => item.Enabled).Select(item => item.GetBotCommand()).ToList();

        }

        public IList<IBotCommand> GetBotCommands()
        {
            return _commandManagementItems.Select(item => item.GetBotCommand()).ToList();

        }

        private void RegisterCallbacks()
        {
            _client.OnLog += Client_OnLog;
            _client.OnMessageReceived += Client_OnMessageReceived;
            _client.OnLog += PingCheck;
        }

        private void PingCheck(object sender, OnLogArgs e)
        {
            if(e.Data.Contains("PING :"))
            {
                _logger.LogInformation("Reseting PING timeout.");
                _timeoutTimer.Stop();
                _timeoutTimer.Start();
            }
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string baseCommand = e.ChatMessage.Message.Split(" ").FirstOrDefault();
            if(string.IsNullOrWhiteSpace(baseCommand)){return;}
            string message;
            foreach (var command in GetEnabledBotCommands())
            {
                if(command.ShouldRun(baseCommand))
                {
                    message = command.Run(e.ChatMessage, new TwitchChatCommand(e.ChatMessage.Message));
                    _logger.LogInformation(message);
                    _client.SendMessage(e.ChatMessage.Channel, message);
                }
            }
        }

        public IList<CommandManagementItem> GetBotCommandOptions()
        {
            return _commandManagementItems;
        }

        public bool IsConnected()
        {
            return _client.IsConnected;
        }
    }
}