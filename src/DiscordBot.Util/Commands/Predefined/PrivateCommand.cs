using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Attributes;

namespace DiscordBot.Util.Commands.Predefined
{
    /// <summary>
    /// Commands that opens a private DM channel with the user who triggered this command
    /// </summary>
    [CommandVerb("private", Description = "Creates a private dm channel between you and the bot. This command can only be called in public channels")]
    internal sealed class PrivateCommand : ICommand
    {
        public async Task Execute(IMessage rawMessage)
        {
            await rawMessage.Channel.SendMessageAsync($"{rawMessage.Author.Mention} i will sent you a dm shortly :innocent:");
            var privateChannel = await rawMessage.Author.GetOrCreateDMChannelAsync();
            await privateChannel.SendMessageAsync("Here you can tell me all your secrets :heart_eyes:");
        }
    }
}
