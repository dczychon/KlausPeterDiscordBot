using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Filter;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace DiscordBot.Util.Commands
{
    /// <summary>
    /// Handles all messages that the bot receives
    /// </summary>
    internal sealed class NewMessageHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandContainer _commands;
        private readonly CommandFilterChecker _filterChecker;

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();


        public NewMessageHandler(IServiceProvider serviceProvider, ICommandContainer commands, IEnumerable<CommandFilter> globalCommandFilters)
        {
            _serviceProvider = serviceProvider;
            _commands = commands;
            _filterChecker = new CommandFilterChecker(globalCommandFilters);
        }


        public async Task HandleMessage(IMessage message)
        {
            if (message.Source != MessageSource.User)
            {
                //Ignore all messages that are not from users
                return;
            }

            try
            {
                if (message.Content.StartsWith("!") && message.Content.Length > 1)
                {
                    _logger.Info("Processing message {0} from \"{1}\" in \"{2}\": \"{3}\"", message.Id, message.Author?.Username, message.Channel.Name, message.Content);

                    string[] msg = message.Content.Substring(1).Split(' ');

                    if (_commands.TryGetCommandType(msg[0], out Type iCommandType))
                    {
                        if (_filterChecker.ExecutionAllowed(iCommandType, message, _serviceProvider, out FilterScope? denyScope))
                        {
                            ICommand command = (ICommand)ActivatorUtilities.CreateInstance(_serviceProvider, iCommandType);

                            ArgumentsMapper mapper = new ArgumentsMapper(msg.Skip(1).ToArray(), command);
                            try
                            {
                                command = mapper.MapToInstance();
                            }
                            catch (CommandArgumentException cmdex)
                            {
                                await message.Channel.SendMessageAsync($":warning: {message.Author.Mention}, {cmdex.Message}. Type \"!help {msg[0]}\" for help");
                                return;
                            }

                            _logger.Debug("Message {0} from {1} is handled by {2}", message.Id, message.Author?.Username, iCommandType.FullName);
                            try
                            {
                                await command.Execute(message);
                            }
                            catch (Exception e)
                            {
                                _logger.Error(e,"Error in command execution for {0}. Message was: \"{1}\"", iCommandType.FullName, message.Content);
                                await ReportCommandHandelingError(message);
                            }
                        }
                        else
                        {
                            if (denyScope != FilterScope.Global)
                            {
                                _logger.Info("Execution of command \"{0}\" from {1} in channel {2} is not allowed", message.Content, message.Author?.Username, message.Channel?.Name);
                                await message.Channel.SendMessageAsync("What you are planning to do is not allowed here! :astonished::point_up:");
                            }
                        }
                    }
                    else
                    {
                        _logger.Info("Found no command handler for {0} in container", msg[0]);
                        if (message.IsPrivateMessage())
                        {
                            await message.Channel.SendMessageAsync("I don´t know what to do with this command :thinking:. What I do know, is that you can find help with the !help command! Isn´t that cool? :ok_hand:");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in HandleMessage() for message {0} with content: \"{1}\"", message.Id, message.Content);
                await ReportCommandHandelingError(message);
            }
        }

        /// <summary>
        /// Sends a message to the source <see cref="SocketChannel"/> that during message processing an error occured
        /// </summary>
        /// <param name="message">Source <see cref="SocketMessage"/></param>
        /// <returns></returns>
        private async Task ReportCommandHandelingError(IMessage message)
        {
            await message.Channel.SendMessageAsync(":robot: Oops! Stupid robot couldn't process this message :face_palm:" + Environment.NewLine + $"`{message.Content}`");
        }
    }
}
