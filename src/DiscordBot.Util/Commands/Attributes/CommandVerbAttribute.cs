using System;

namespace DiscordBot.Util.Commands.Attributes
{
    /// <summary>
    /// Defines a command verb for a class, that implements the <see cref="ICommand"/> interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CommandVerbAttribute : Attribute
    {
        /// <summary>
        /// Verb that triggers the command when prefixed with a ! 
        /// </summary>
        public string Verb { get; }

        /// <summary>
        /// Description about what the command is doing for help text
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Specifies iof the command is hidden when printing out all supported commands
        /// </summary>
        public bool Hidden { get; set; } = false;

        public CommandVerbAttribute(string verb)
        {
            Verb = verb;
        }
    }
}
