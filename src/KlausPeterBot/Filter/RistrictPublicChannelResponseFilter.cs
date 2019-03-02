using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util;
using DiscordBot.Util.Commands.Filter;

namespace KlausPeterBot.Filter
{
    /// <summary>
    /// Restricts the execution of commands to multiple public channels
    /// </summary>
    class RistrictPublicChannelResponseFilter : MultipleIdFilter
    {
        public RistrictPublicChannelResponseFilter(IEnumerable<ulong> allowedChanneldsIds) : base(allowedChanneldsIds) { }

        public override bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider)
        {
            if (message.IsPrivateMessage() || !_ids.Any())
            {
                return true;
            }

            return _ids.Any(c => c == message.Channel.Id);
        }
    }
}
