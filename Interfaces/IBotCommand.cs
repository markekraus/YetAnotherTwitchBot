using System.Collections.Generic;
using TwitchLib.Client.Models;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Interfaces
{
    public interface IBotCommand
    {
        string Run(ChatMessage ChatMessage, TwitchChatCommand ChatCommand);
        string GetCommandName();
        string GetCommandDescription();
        bool ShouldRun(string Command);
    }
}