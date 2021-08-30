namespace YetAnotherTwitchBot.Interfaces
{
    public interface ISettingsHelper
    {
         void AddOrUpdateAppSetting<T>(string sectionPathKey, T value);
    }
}