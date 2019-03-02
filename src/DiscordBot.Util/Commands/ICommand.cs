using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Util.Commands
{
    public interface ICommand
    {
        Task Execute(IMessage rawMessage);
    }
}
