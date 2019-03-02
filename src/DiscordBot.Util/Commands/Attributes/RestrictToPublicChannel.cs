using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Filter;

namespace DiscordBot.Util.Commands.Attributes
{
    /// <summary>
    /// Restricts the execution of a command to public channels (no DM) only
    /// </summary>
    public sealed class RestrictToPublicChannel : CommandFilter
    {
        public override bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider)
        {
            return !message.IsPrivateMessage();
        }
    }
}
