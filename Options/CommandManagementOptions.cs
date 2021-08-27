using System.Collections.Generic;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Options
{
    public class CommandManagementOptions
    {
        public const string Section = "CommandManagement";
        public IList<CommandManagementItem> CommandSettings { get; set; }
    }
}