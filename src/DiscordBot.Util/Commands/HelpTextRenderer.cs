using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DiscordBot.Util.Commands.Attributes;

namespace DiscordBot.Util.Commands
{
    /// <summary>
    /// Generates a help text for a command
    /// </summary>
    public class HelpTextRenderer
    {
        protected readonly Type CommandType;

        protected readonly CommandVerbAttribute CommandVerb;

        protected readonly IReadOnlyList<CommandArgumentAttribute> AllOptions;

        public HelpTextRenderer(Type commandType)
        {
            if (commandType.GetCustomAttribute<CommandVerbAttribute>() == null)
            {
                throw new ArgumentException($"Provided type must have the {typeof(CommandVerbAttribute).FullName}", nameof(commandType));
            }

            CommandType = commandType;
            CommandVerb = commandType.GetCustomAttribute<CommandVerbAttribute>();
            AllOptions = commandType.GetProperties().Select(p => p.GetCustomAttribute<CommandArgumentAttribute>()).OrderBy(x => x.Index).ToList();
        }

        public virtual StringBuilder GenerateSyntaxHelp()
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append($"Syntax: **!{CommandVerb.Verb}**");

            foreach (var option in AllOptions)
            {
                if (option.Required)
                {
                    strBuilder.Append($" [{option.Name}]");
                }
                else
                {
                    strBuilder.Append($" ({option.Name})");
                }
            }

            return strBuilder;
        }

        public virtual StringBuilder GenerateArgumentsHelp()
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var option in AllOptions)
            {
                strBuilder.AppendLine($"{option.Name}\t\t -> {option.HelpText}");
                strBuilder.AppendLine($"\tRequired: {option.Required}");
                strBuilder.AppendLine($"\tDefault: {option.DefaultValue?.ToString() ?? "null"}");
                strBuilder.AppendLine();
            }

            return strBuilder;
        }

        public virtual StringBuilder GenerateHeaderHelp()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"**!{CommandVerb.Verb}** \t- {CommandVerb.Description}");
            return strBuilder;
        }

        public virtual string GetHelp()
        {
            StringBuilder strBuilder = new StringBuilder();
            
            strBuilder.Append(GenerateHeaderHelp());
            strBuilder.AppendLine();
            strBuilder.Append(GenerateSyntaxHelp());
            strBuilder.AppendLine();
            strBuilder.AppendLine();
            strBuilder.Append(GenerateArgumentsHelp());

            return strBuilder.ToString();
        }
    }
}
