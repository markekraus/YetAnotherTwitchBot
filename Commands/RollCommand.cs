using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Commands
{
    public class RollCommand : IBotCommand
    {
        public const string PrimaryCommand = "!roll";
        public const string CommandDescription = "A dice roll command for D&D style dice.";
        private Regex CommandRegex = new Regex("!roll|!r$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private ILogger<RollCommand> _logger;
        private Regex NDMPattern = new Regex("^([0-9]+)d([0-9]+)([+-][0-9]+){0,1}$",RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex DMPattern = new Regex("^d([0-9]+)([+-][0-9]+){0,1}$",RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex MPattern = new Regex("^([0-9]+)([+-][0-9]+){0,1}$",RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public RollCommand(ILogger<RollCommand> Logger)
        {
            _logger = Logger;
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
            string message;
            int numberOfDice;
            int maxDiceSides;
            int offset;
            if(!TryParseParameter(ChatCommand, out numberOfDice, out maxDiceSides, out offset))
            {
                message = $"@{ChatMessage.DisplayName} invalid command. Examples: '!roll 3d6', '!roll 3d6+5', '!roll 3d6-5', '!roll d6', '!roll 6'.";
            }
            else
            {
                var results = Roll(numberOfDice, maxDiceSides, offset);
                message = $"@{ChatMessage.DisplayName} Results: {results}";
            }

            return message;
        }

        public bool ShouldRun(string Command)
        {
            return CommandRegex.IsMatch(Command);
        }

        private bool TryParseParameter(TwitchChatCommand command, out int numberOfDice, out int maxDiceSides, out int offset)
        {
            offset = 0;
            numberOfDice = 0;
            maxDiceSides = 0;
            // match !roll
            if(!command.HasParameters)
            {
                numberOfDice = 1;
                maxDiceSides = 6;
                return true;
            }

            var Parameter = command.Parameters[0];
            // match !roll 3d6
            var match = NDMPattern.Match(Parameter);
            if(match.Success && int.TryParse(match.Groups[1].Value,out numberOfDice) && int.TryParse(match.Groups[2].Value, out maxDiceSides) && numberOfDice > 0 && maxDiceSides > 0)
            {
                if (match.Groups[3].Success && !int.TryParse(match.Groups[3].Value, out offset))
                {
                    return false;
                }
                return true;
            }

            // match !roll d6
            match = DMPattern.Match(Parameter);
            if(match.Success && int.TryParse(match.Groups[1].Value,out maxDiceSides) && maxDiceSides > 0)
            {
                numberOfDice = 1;
                if (match.Groups[2].Success && !int.TryParse(match.Groups[2].Value, out offset))
                {
                    return false;
                }
                return true;
            }

            // match !roll 6
            match = MPattern.Match(Parameter);
            if(match.Success && int.TryParse(match.Groups[1].Value,out maxDiceSides)  && maxDiceSides > 0)
            {
                numberOfDice = 1;
                if (match.Groups[2].Success && !int.TryParse(match.Groups[2].Value, out offset))
                {
                    return false;
                }
                return true;
            }

            numberOfDice = 0;
            maxDiceSides = 0;
            return false;
        }

        private string Roll(int numberOfDice, int maxDiceSides, int offset)
        {
            var random = new Random();
            var sb = new StringBuilder();
            int total = 0;
            for (int i = 0; i < numberOfDice; i++)
            {
                var roll = random.Next(1,maxDiceSides);
                total += roll;
                sb.Append(roll);
                sb.Append(" ");
            }
            total = total + offset;
            sb.Append("Total: ");
            sb.Append(total);

            return sb.ToString();
        }
    }
}