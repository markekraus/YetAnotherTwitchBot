using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YetAnotherTwitchBot.Services
{
    public class SettingsHelper
    {
        private IWebHostEnvironment _env;
        public SettingsHelper(IWebHostEnvironment Env)
        {
            _env = Env;
        }
        public void AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
        {
            string fileName = "appsettings.json";
            if(!_env.IsProduction())
            {
                fileName = $"appsettings.{_env.EnvironmentName}.json";
            }
            var filePath = Path.Combine(_env.ContentRootPath, fileName);
            string json = File.ReadAllText(filePath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            SetValueRecursively(sectionPathKey, jsonObj, value);
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(filePath, output);
        }

        private void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            // split the string at the first ':' character
            var remainingSections = sectionPathKey.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the procress, moving down the tree
                var nextSection = remainingSections[1];
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
            }
            else
            {
                // we've got to the end of the tree, set the value
                jsonObj[currentSection] = JToken.FromObject(value); 
            }
        }
    }
}