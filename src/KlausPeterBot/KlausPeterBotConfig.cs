using System;
using System.Collections.Generic;
using System.IO;
using DiscordBot.Util;
using Newtonsoft.Json;
using NLog;

namespace KlausPeterBot
{
    public sealed class KlausPeterBotConfig : BotConfiguration
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private const string BotConfigPathEnvName = "KP_BOTCONFIG";


        public string DiscordToken { get; set; }

        public string FortniteTrackerToken { get; set; }

        public IEnumerable<ulong> RespondeInChannels { get; set; } = Array.Empty<ulong>();

        public IEnumerable<ulong> RespondeToUsers { get; set; } = Array.Empty<ulong>();

        public static KlausPeterBotConfig LoadFile()
        {
            string fileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "botconfig.json");
            string envValue = Environment.GetEnvironmentVariable(BotConfigPathEnvName);

            if (!string.IsNullOrWhiteSpace(envValue) && File.Exists(envValue))
            {
                //File from env exists
                fileLocation = envValue;
            }
            else
            {
                //If no env variable specified, check if local botconfig exists
                if (!File.Exists(fileLocation))
                {
                    throw new Exception($"No bot configuration file found. Define path with {BotConfigPathEnvName} environment variable");
                }
            }

            return LoadFile(fileLocation);
        }
        public static KlausPeterBotConfig LoadFile(string path)
        {
            try
            {
                _logger.Info("Loading configuration from {0}", path);
                return JsonConvert.DeserializeObject<KlausPeterBotConfig>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while loading bot configuration file from \"{path}\"", ex);
            }
        }
    }
}
