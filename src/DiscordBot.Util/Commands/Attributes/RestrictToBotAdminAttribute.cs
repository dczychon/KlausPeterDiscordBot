using Discord;
using Discord.WebSocket;
using DiscordBot.Util;
using DiscordBot.Util.Commands.Filter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Util.Commands.Attributes
{
    /// <summary>
    /// Restricts the usage of a command to only bot admins
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RestrictToBotAdminAttribute : CommandFilter
    {
        public override bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<BotConfiguration>();
            return config.BotAdministrators.Any(u => u == message.Author.Id);
        }
    }
}
