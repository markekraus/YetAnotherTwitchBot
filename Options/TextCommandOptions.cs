using System.Collections.Generic;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Options
{
    public class TextCommandOptions
    {
        public const string Section = "TextCommands";
        public IList<TextCommand> Commands { get; set; }
    }
}