using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Attributes;

namespace DiscordBot.Util.Commands.Predefined
{
    /// <summary>
    /// Provides a way to get help about bot commands
    /// </summary>
    [CommandVerb("help", Description = "Prints usage for a command")]
    public sealed class HelpCommand : ICommand
    {
        private readonly ICommandContainer _commands;

        [CommandArgument(0, "Command name", HelpText = "Name of the command to print help for")]
        public string Command { get; set; }

        public HelpCommand(ICommandContainer commands)
        {
            _commands = commands;
        }

        public async Task Execute(IMessage rawMessage)
        {
            if (string.IsNullOrWhiteSpace(Command))
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendLine(":information_source: __**Here are all supported commands:**__ :information_source:");
                strBuilder.AppendLine();

                foreach (Type commandType in _commands.OrderBy(x => x.Name))
                {
                    var verbAttr = commandType.GetCustomAttribute<CommandVerbAttribute>();
                    if (verbAttr != null && !verbAttr.Hidden)
                    {
                        HelpTextRenderer helpRenderer = new HelpTextRenderer(commandType);
                        strBuilder.Append(helpRenderer.GenerateHeaderHelp());
                        strBuilder.AppendLine();
                    }
                }

                await rawMessage.Channel.SendMessageAsync(strBuilder.ToString());
            }
            else
            {
                if (_commands.TryGetCommandType(Command, out Type commandType))
                {
                    HelpTextRenderer helpRenderer = new HelpTextRenderer(commandType);
                    await rawMessage.Channel.SendMessageAsync(helpRenderer.GetHelp());
                }
                else
                {
                    await rawMessage.Channel.SendMessageAsync($"\"{Command}\" is an unknown command. Type !help to list all available commands!");
                }
            }
        }
    }
}
