using System.Collections.Generic;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Interfaces
{
    public interface IBotCommandService
    {
        void StartIrc();
        void StopIrc();
        IList<IBotCommand> GetBotCommands();
        IList<IBotCommand> GetEnabledBotCommands();
        IList<CommandManagementItem> GetBotCommandOptions();
        bool IsConnected();
    }
}