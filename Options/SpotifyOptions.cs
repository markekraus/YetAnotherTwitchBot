using SpotifyAPI.Web;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Options
{
    public class SpotifyOptions
    {
        public const string Section = "Spotify";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public SpotifyToken Token { get; set; }
        public bool Enabled { get; set; } = false;
    }
}