using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordBot.Util;
using DiscordBot.Util.Commands.Filter;
using DiscordBot.Util.Logging;
using KlausPeterBot.Filter;
using Microsoft.Extensions.DependencyInjection;

namespace KlausPeterBot
{
    class KlausPeterProgram : BotProgram
    {
        /// <summary>
        /// Holds the bot configuration
        /// </summary>
        private static KlausPeterBotConfig _botConfig;

        /// <summary>
        /// Entry point for KlausPeterBot is called by dotnet runtime
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            BotLoggingManager.InitLogging();
            _botConfig = KlausPeterBotConfig.LoadFile();
            await new KlausPeterProgram().Run(_botConfig.DiscordToken);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<HttpClient>();
            services.AddSingleton(_botConfig);
            services.AddSingleton<BotConfiguration>(_botConfig);    //Regsiter again as BotConfiguration Type
        }

        protected override IEnumerable<CommandFilter> ConfigureGlobalFilter()
        {
            yield return new RistrictPublicChannelResponseFilter(_botConfig.RespondeInChannels);
            yield return new RestrictUserResponseFilter(_botConfig.RespondeToUsers);
        }
    }
}
