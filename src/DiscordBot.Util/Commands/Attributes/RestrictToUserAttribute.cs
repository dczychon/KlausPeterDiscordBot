using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Filter;

namespace DiscordBot.Util.Commands.Attributes
{
    /// <summary>
    /// Restricts the usage of an command to a specific user
    /// </summary>
    [Obsolete("Try prevent hardcoded discord ids")]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class RestrictToUserAttribute : CommandFilter
    {
        /// <summary>
        /// ID of the user who is allowed to use this command
        /// </summary>
        public ulong UserId { get; }

        public RestrictToUserAttribute(ulong userId)
        {
            UserId = userId;
        }

        public override bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider)
        {
            return message.Author.Id == UserId;
        }
    }
}
