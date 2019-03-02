using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Attributes;

namespace DiscordBot.Util.RuntimeDebugging
{
    [RestrictToPrivateDm]
    [RestrictToBotAdmin]
    [CommandVerb("inspect-dsc", Description = "Allows the inspection of the DiscordSocketClient at runtime", Hidden = true)]
    internal sealed class InspectDscCommand : ICommand
    {
        private readonly DiscordSocketClient _discordSocketClient;

        [CommandArgument(0, nameof(Path), Required = true, HelpText = "Path to property or field from DiscordSocketClient as root or 'print' to show tree")]
        public string Path { get; set; }

        public InspectDscCommand(DiscordSocketClient discordSocketClient)
        {
            _discordSocketClient = discordSocketClient;
        }

        public async Task Execute(IMessage rawMessage)
        {
            TypeInspector<DiscordSocketClient> inspector = new TypeInspector<DiscordSocketClient>(_discordSocketClient);
            if (Path == "print")
            {
                await rawMessage.Channel.SendMessageAsync(inspector.PrettyPrint());
            }
            else
            {
                try
                {
                    await rawMessage.Channel.SendMessageAsync(inspector.GetValue(Path).ToString());
                }
                catch (Exception e)
                {
                    await rawMessage.Channel.SendMessageAsync(":warning: " + e.Message);
                }
            }
        }
    }
}
