using YetAnotherTwitchBot.Options;

namespace YetAnotherTwitchBot.Models
{
    public class BotSettings
    {
        public TwitchOptions Twitch {get; set;} = new TwitchOptions();
        public SpotifyOptions Spotify { get; set; } = new SpotifyOptions();
    }
}