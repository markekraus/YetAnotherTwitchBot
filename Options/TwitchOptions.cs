using System.Collections.Generic;

namespace YetAnotherTwitchBot.Options
{
    public class TwitchOptions
    {
        public const string Section = "Twitch";

        public string BotIrcPassword { get; set; }
        public string BotUsername { get; set; }
        public string StreamerUsername { get; set; }
        public IList<string> Channels { get; set; }
        public bool Enabled { get; set; } = false;
    }
}