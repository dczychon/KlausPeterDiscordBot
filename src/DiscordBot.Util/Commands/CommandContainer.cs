using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DiscordBot.Util.Commands.Attributes;
using NLog;

namespace DiscordBot.Util.Commands
{
    /// <summary>
    /// Basic <see cref="ICommandContainer"/> implementation that stores <see cref="ICommand"/> types
    /// </summary>
    public class CommandContainer : ICommandContainer
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        internal volatile Dictionary<string, Type> _commandsDictionary = new Dictionary<string, Type>();
        internal readonly object _lock = new object();

        /// <summary>
        /// Adds a command handler to the container
        /// </summary>
        /// <typeparam name="T">Command handler type that implements the <see cref="ICommand"/> interface</typeparam>
        public void AddCommandHandler<T>() where T : ICommand
        {
            AddCommandHandler(typeof(T));
        }

        /// <summary>
        /// Adds a command handler to the container
        /// </summary>
        /// <param name="commandType">Command handler type that implements the <see cref="ICommand"/> interface and has the <see cref="CommandVerbAttribute"/> supplied</param>
        public void AddCommandHandler(Type commandType)
        {
            IsValidCommandType(commandType);

            lock (_lock)
            {
                _logger.Debug("Adding \"{0}\" to command container", commandType.FullName);
                _commandsDictionary.Add(commandType.GetCustomAttribute<CommandVerbAttribute>().Verb, commandType);
            }
        }

        /// <summary>
        /// Checks if the given Type is a valid <see cref="ICommand"/> and has the <see cref="CommandVerbAttribute"/> supplied
        /// </summary>
        /// <param name="type"></param>
        private static void IsValidCommandType(Type type)
        {
            if (!typeof(ICommand).IsAssignableFrom(type))
            {
                _logger.Fatal("The type {0} must implement the {1} interface to be used as command", type.FullName, typeof(ICommand).FullName);
                throw new Exception();
            }

            if (type.GetCustomAttribute<CommandVerbAttribute>() == null)
            {
                _logger.Fatal("The type {0} must implement the {1} attribute to be used as command", type.FullName, typeof(CommandVerbAttribute).FullName);
                throw new Exception();
            }
        }

        /// <summary>
        /// Trys to get the type that handles the specified command
        /// </summary>
        /// <param name="command">Command for which a command handler type is requested</param>
        /// <param name="type">If found, type which can handle this command</param>
        /// <returns></returns>
        public bool TryGetCommandType(string command, out Type type)
        {
            return _commandsDictionary.TryGetValue(command, out type);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return _commandsDictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
