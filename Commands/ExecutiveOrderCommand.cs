using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class ExecutiveOrderCommand : IBotCommand
    {
        public const string PrimaryCommand = "!executiveorder";
        public const string CommandDescription = "Retrieves a random executive order.";
        private Regex CommandRegex = new Regex("!executiveorder|!eo", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private ILogger<ExecutiveOrderCommand> _logger;
        IExecutiveOrderService _service;
        public ExecutiveOrderCommand(
            ILogger<ExecutiveOrderCommand> Logger,
            IExecutiveOrderService Service)
        {
            _logger = Logger;
            _service = Service;
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
            var order = _service.GetRandom();
            return $"EO {order.ExecutiveOrderNumber}: \"{order.Title}\" by President {order.President}. Published {order.PublicationDate.ToString("MMMM dd, yyyy")}";
        }

        public bool ShouldRun(string Command)
        {
            return CommandRegex.IsMatch(Command);
        }
    }
}