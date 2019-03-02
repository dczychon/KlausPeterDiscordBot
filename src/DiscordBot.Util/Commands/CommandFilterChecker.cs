using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Attributes;
using DiscordBot.Util.Commands.Filter;
using NLog;

namespace DiscordBot.Util.Commands
{
    internal sealed class CommandFilterChecker
    {
        private readonly IEnumerable<CommandFilter> _globalFilter;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public CommandFilterChecker(IEnumerable<CommandFilter> globalFilter)
        {
            _globalFilter = globalFilter;
        }

        public bool ExecutionAllowed(Type commandType, IMessage message, IServiceProvider serviceProvider, out FilterScope? denyScope)
        {
            foreach (CommandFilter commandFilter in _globalFilter)
            {
                if (!commandFilter.CanExecuteInContext(message, serviceProvider))
                {
                    _logger.Debug("Global filter {0} denys the execution of the current command for {1}", commandFilter.GetType().FullName, message.Author.Username);
                    denyScope = FilterScope.Global;     //A global filter denys the execution
                    return false;
                }
            }

            var allFilter = commandType.GetCustomAttributes().Where(x => x is CommandFilter).Cast<CommandFilter>();
            foreach (CommandFilter commandFilter in allFilter)
            {
                if (!commandFilter.CanExecuteInContext(message, serviceProvider))
                {
                    _logger.Debug("Instance filter {0} denys the execution of the current command for {1}", commandFilter.GetType().FullName, message.Author.Username);
                    denyScope = FilterScope.Command;    //A command specific filter denys the execution
                    return false;
                }
            }

            denyScope = null;
            return true;
        }
    }
}
