using System.Collections.Generic;
using YetAnotherTwitchBot.Models;
using YetAnotherTwitchBot.Options;

namespace YetAnotherTwitchBot.Interfaces
{
    public interface ITextCommandService
    {
        void AddTextCommand(TextCommand Command);
        void RemoveTextCommand(string CommandName);
        void RemoveTextCommand(TextCommand Command);
        IList<TextCommand> GetTextCommands();
        void UpdateSettings(TextCommandOptions Options);
    }
}