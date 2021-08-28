using System.Collections.Generic;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Options
{
    public class TextCommandsOptions
    {
        public const string Section = "TextCommands";
        public IList<TextCommandsItem> Commands { get; set; }
        public IList<TextCommandAdminUser> AdminUsers { get; set; } = new List<TextCommandAdminUser>();
    }
}