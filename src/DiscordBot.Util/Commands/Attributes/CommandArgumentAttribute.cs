using System;

namespace DiscordBot.Util.Commands.Attributes
{
    /// <summary>
    /// Defines a public property as an command argument
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CommandArgumentAttribute : Attribute
    {
        /// <summary>
        /// Index were the arguments value should be specified
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Name of the argument for help text purposes
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Help text for what the value is for
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Default value if the user didn´t supplied a value
        /// </summary>
        public object DefaultValue { get; set; } = null;

        /// <summary>
        /// Defines if the value is required for the command to run
        /// </summary>
        public bool Required { get; set; }
        
        public CommandArgumentAttribute(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}
