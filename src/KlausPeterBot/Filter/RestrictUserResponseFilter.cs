using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util;
using DiscordBot.Util.Commands.Filter;

namespace KlausPeterBot.Filter
{
    /// <summary>
    /// Restricts the execution of commands to only defined users
    /// </summary>
    class RestrictUserResponseFilter : MultipleIdFilter
    {
        public RestrictUserResponseFilter(IEnumerable<ulong> allowedUsersIds) : base(allowedUsersIds) { }

        public override bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider)
        {
            if (message.IsPrivateMessage() || !_ids.Any())
            {
                return true;
            }

            return _ids.Any(c => c == message.Author.Id);
        }
    }
}
