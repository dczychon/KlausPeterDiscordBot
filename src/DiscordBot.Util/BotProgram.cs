using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Filter;
using DiscordBot.Util.Commands.Predefined;
using DiscordBot.Util.Logging;
using DiscordBot.Util.RuntimeDebugging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot.Util
{
    /// <summary>
    /// A universal base class for discord bots
    /// </summary>
    public abstract class BotProgram
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static readonly ILogger _discordLogger = LogManager.GetLogger("DiscordSocketClient");


        /// <summary>
        /// UTC Time when the bot was started
        /// </summary>
        public static DateTime BotStartupTimeUtc { get; } = DateTime.UtcNow;


        private static readonly object _lastDiscordConnectionLostLock = new object();
        private static DateTime? _lastDiscordConnectionLostUtc = null;

        /// <summary>
        /// Last time (in UTC) when the bot lost the connection to discord
        /// </summary>
        public static DateTime? LastDiscordConnectionLostUtc
        {
            get
            {
                lock (_lastDiscordConnectionLostLock)
                {
                    return _lastDiscordConnectionLostUtc;
                }
            }

            private set
            {
                lock (_lastDiscordConnectionLostLock)
                {
                    _lastDiscordConnectionLostUtc = value;
                }
            }
        }

        /// <summary>
        /// Client to interact with discord
        /// </summary>
        private readonly DiscordSocketClient _discordClient = new DiscordSocketClient();

        /// <summary>
        /// Handles the MessageReceived event from <see cref="DiscordSocketClient"/>
        /// </summary>
        private readonly NewMessageHandler _newMessageHandler;

        protected BotProgram()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _logger.Info("Starting Bot v{0} on {1}({2})", Assembly.GetEntryAssembly().GetName().Version, Environment.MachineName, Environment.OSVersion.VersionString);

            //Configure Dependency Injection
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceCollection.AddSingleton(_discordClient);
            serviceCollection.TryAddSingleton(new BotConfiguration());  //Add default bot configuration if not already added
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            //Configure Command Container
            BotConfiguration config = serviceProvider.GetService<BotConfiguration>();
            ICommandContainer container = BuildCommandContainer();
            container.AddCommandHandler<HelpCommand>();
            container.AddCommandHandler<PrivateCommand>();
            container.AddCommandHandler<PingCommand>();
            if (config.EnableDebugCommandsForAdmins)
            {
                container.AddCommandHandler<InspectDscCommand>();
                container.AddCommandHandler<InfoCommand>();
            }
            else
            {
                _logger.Debug("Debug commands are disabled. Not adding them to command container");
            }
            _logger.Info("Added {0} command(s) to container", container.Count());
            serviceCollection.AddSingleton(container);
            serviceProvider = serviceCollection.BuildServiceProvider(); //Build ServiceProvider agein with ICommandContainer

            //Configure global filter
            IEnumerable<CommandFilter> globalFilter = ConfigureGlobalFilter().ToList();
            foreach (CommandFilter commandFilter in globalFilter)
            {
                _logger.Debug("Loaded global filter \"{0}\"", commandFilter.GetType().FullName);
            }
            _logger.Info("Added {0} global filter(s)", globalFilter.Count());

            //Configure Event Handler
            _newMessageHandler = new NewMessageHandler(serviceProvider, container, globalFilter);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            _logger.Fatal(ex, "Unhandled exception occurred");
            Environment.FailFast(ex.Message, ex);
        }

        /// <summary>
        /// Starts the bot
        /// </summary>
        /// <param name="token">Discord bot token</param>
        /// <returns></returns>
        public async Task Run(string token)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            _discordClient.Log += _discordClient_Log;

            using (new EntryExitLog(_logger.Debug, "Logging in to Discord"))
            {
                await _discordClient.LoginAsync(TokenType.Bot, token);
            }

            _logger.Debug("Calling StartAsync()");
            await _discordClient.StartAsync();

            _discordClient.Disconnected += (ex) =>
            {
                LastDiscordConnectionLostUtc = DateTime.UtcNow; //Update last connection lost value on disconnect
                return Task.CompletedTask;
            };

            _discordClient.Ready += _discordClient_Ready;
            _discordClient.MessageReceived += _newMessageHandler.HandleMessage;

            await Task.Delay(-1);
        }

        /// <summary>
        /// Performs actions when the <see cref="DiscordSocketClient"/> connection is ready
        /// </summary>
        /// <returns></returns>
        private Task _discordClient_Ready()
        {
            var guilds = _discordClient.Guilds;
            _logger.Info("Bot is present on {0} server(s)", guilds.Count);
            foreach (SocketGuild guild in guilds)
            {
                _logger.Info("\t-> {0} - {1}", guild.Id, guild.Name);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles <see cref="DiscordSocketClient"/> logs
        /// </summary>
        /// <param name="arg">Log from <see cref="DiscordSocketClient"/></param>
        /// <returns></returns>
        private Task _discordClient_Log(LogMessage arg)
        {
            LogLevel level;

            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    level = LogLevel.Fatal;
                    break;
                case LogSeverity.Error:
                    level = LogLevel.Error;
                    break;
                case LogSeverity.Warning:
                    level = LogLevel.Warn;
                    break;
                case LogSeverity.Info:
                    level = LogLevel.Info;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    level = LogLevel.Debug;
                    break;

                default:
                    level = LogLevel.Info;
                    break;
            }

            _discordLogger.Log(level, arg.Exception, "{0}\t{1}", arg.Source, arg.Message);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Closes the connection to discord on application exit
        /// </summary>
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            using (new EntryExitLog(_logger.Info, "Shutting down bot"))
            {
                Task.Run(async () =>
                {
                    await _discordClient.StopAsync();
                    await _discordClient.LogoutAsync();
                }).GetAwaiter().GetResult();
            }

            LogManager.Shutdown();
        }

        /// <summary>
        /// Override this method to define your commands. By default all types that implement the ICommand interface in the entry assembly are loaded
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Type> DiscoverCommands()
        {
            return Assembly.GetEntryAssembly().GetTypes().Where(t => typeof(ICommand).IsAssignableFrom(t));
        }

        /// <summary>
        /// Override this method to define a custom <see cref="ICommandContainer"/>
        /// </summary>
        /// <returns></returns>
        protected virtual ICommandContainer BuildCommandContainer()
        {
            CommandContainer container = new CommandContainer();

            List<Type> discovered = DiscoverCommands().ToList();
            _logger.Info("Discovered {0} command(s)", discovered.Count);

            foreach (Type type in discovered)
            {
                container.AddCommandHandler(type);
            }

            return container;
        }

        /// <summary>
        /// Override this method to add services to the service collection
        /// </summary>
        protected virtual void ConfigureServices(IServiceCollection services) { }

        /// <summary>
        /// Override this method to add global message filter
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<CommandFilter> ConfigureGlobalFilter()
        {
            return Array.Empty<CommandFilter>();
        }
    }
}
