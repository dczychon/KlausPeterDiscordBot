using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Filter;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Util.Commands.Attributes
{
    /// <summary>
    /// Restricts the execution of a command to private DM channels only
    /// </summary>
    public sealed class RestrictToPrivateDmAttribute : CommandFilter
    {
        public override bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<BotConfiguration>();

            if (config.RespondeOnlyToAdminsInPrivateChannels)
            {
                return config.BotAdministrators.Any(u => u == message.Author.Id) && message.IsPrivateMessage();
            }

            return message.IsPrivateMessage();
        }
    }
}
