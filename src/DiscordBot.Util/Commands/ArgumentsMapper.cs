using System;
using System.Reflection;
using DiscordBot.Util.Commands.Attributes;

namespace DiscordBot.Util.Commands
{
    /// <summary>
    /// Maps command arguments to public properties
    /// </summary>
    internal sealed class ArgumentsMapper
    {
        private readonly string[] _args;
        private readonly ICommand _commandHandler;

        /// <summary>
        /// Creates a new <see cref="ArgumentsMapper"/> for the specified <see cref="ICommand"/> instance with supplied arguments
        /// </summary>
        /// <param name="args">Arguments to map to properties</param>
        /// <param name="commandHandler"><see cref="ICommand"/> instance to map to</param>
        public ArgumentsMapper(string[] args, ICommand commandHandler)
        {
            _args = args;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Maps the arguments to the supplied instance
        /// </summary>
        /// <returns><see cref="ICommand"/> instance with arguments mapped to public properties</returns>
        public ICommand MapToInstance()
        {
            PropertyInfo[] allProps = _commandHandler.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in allProps)
            {
                var optionsAttribute = property.GetCustomAttribute<CommandArgumentAttribute>();

                if (optionsAttribute != null)
                {
                    //Property is an command argument
                    if (_args.HasIndex(optionsAttribute.Index))
                    {
                        property.SetValue(_commandHandler, Convert.ChangeType(_args[optionsAttribute.Index], property.PropertyType));   //TODO Handle type conversion errors
                    }
                    else if (optionsAttribute.Required)
                    {
                        throw new CommandArgumentException($"The argument \"{optionsAttribute.Name}\" is required for this command!");
                    }
                    else
                    {
                        property.SetValue(_commandHandler, Convert.ChangeType(optionsAttribute.DefaultValue, property.PropertyType));
                    }
                }
            }

            return _commandHandler;
        }
    }
}
