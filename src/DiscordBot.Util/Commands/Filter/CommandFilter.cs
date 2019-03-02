using System;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Util.Commands.Filter
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public abstract class CommandFilter : Attribute
    {
        public abstract bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider);
    }
}
