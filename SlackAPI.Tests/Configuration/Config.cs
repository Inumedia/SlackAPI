using System;
using System.IO;
using Newtonsoft.Json;

namespace SlackAPI.Tests.Configuration
{
    using File = System.IO.File;

    public class Config
    {
        public SlackConfig Slack { get; set; }

        public static Config GetConfig()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, @"configuration\config.json");
            string json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}