using System.Text.Json.Serialization;
using YetAnotherTwitchBot.Interfaces;

namespace YetAnotherTwitchBot.Models
{
    public class CommandManagementItem
    {
        public string CommandName { get; set; }
        public bool Enabled { get; set; }
        [JsonIgnore]
        private IBotCommand BotCommand;

        public CommandManagementItem()
        {
            
        }
        public CommandManagementItem(IBotCommand Command, bool Enabled = true)
        {
            BotCommand = Command;
            CommandName = Command.GetCommandName();
            this.Enabled = Enabled;
        }

        public void SetBotCommand(IBotCommand Command)
        {
            BotCommand = Command;
        }

        public IBotCommand GetBotCommand()
        {
            return BotCommand;
        }
    }
}