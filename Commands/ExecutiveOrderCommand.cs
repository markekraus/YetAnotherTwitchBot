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
            ExecutiveOrder order;
            if (ChatCommand.HasParameters)
            {
                try
                {
                    order = _service.Get(ChatCommand.Parameters[0]);
                }
                catch (System.Exception)
                {
                    return $"@{ChatMessage.DisplayName} I was unable to find Executive Order {ChatCommand.Parameters[0]}. I can only find EOs {_service.GetMinOrderId()} - {_service.GetMaxOrderId()}.";
                }
            }
            else
            {
                order = _service.GetRandom();
            }
            string date = $". Published on {order.PublicationDate.ToString("MMMM dd, yyyy")}";
            if(!string.IsNullOrWhiteSpace(order.SigningDate))
            {
                try
                {
                    date = DateTime.Parse(order.SigningDate).ToString("MMMM dd, yyyy");
                    date = $" on {date}";
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Unable to convert '{order.SigningDate}' to date for EO {order.ExecutiveOrderNumber}");
                }
            }
            else
            {
                _logger.LogError($"No signing date for ");
            }

            return $"EO {order.ExecutiveOrderNumber}: \"{order.Title}\" signed by President {order.President}{date}";
        }

        public bool ShouldRun(string Command)
        {
            return CommandRegex.IsMatch(Command);
        }
    }
}