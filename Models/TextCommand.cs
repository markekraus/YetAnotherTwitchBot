namespace YetAnotherTwitchBot.Models
{
    public class TextCommand
    {
        public string Command { get; set; }
        public string Template { get; set; }
        public bool Enabled { get; set; } = true;
    }
}