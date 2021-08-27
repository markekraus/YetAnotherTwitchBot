

using System.Text.Json.Serialization;

namespace YetAnotherTwitchBot.Models
{
    public class IssLocation
    {
        public string Message { get; set; }
        public int timestamp { get; set; }
        [JsonPropertyName("iss_position")]
        public IssPosition IssPosition  { get; set; }

        public IssLocation() {}
    }
}