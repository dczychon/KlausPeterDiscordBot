using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Attributes;

namespace DiscordBot.Util.Commands.Predefined
{
    /// <summary>
    /// Simple command to check if the bot is properly configured
    /// </summary>
    [CommandVerb("ping", Description = "Execute this to get a super special response")]
    public sealed class PingCommand : ICommand
    {
        public async Task Execute(IMessage rawMessage)
        {
            await rawMessage.Channel.SendMessageAsync("pong");
        }
    }
}
