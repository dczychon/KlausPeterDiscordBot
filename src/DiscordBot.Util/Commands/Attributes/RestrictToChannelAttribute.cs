using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands.Filter;

namespace DiscordBot.Util.Commands.Attributes
{
    /// <summary>
    /// Restricts the Command to only process messages in this channel. To restrict command to multiple channels, use this attribute multiple times on the same class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RestrictToChannelAttribute : CommandFilter
    {
        /// <summary>
        /// Channel ID of the allowed channel
        /// </summary>
        public ulong ChannelId { get; }

        public RestrictToChannelAttribute(ulong channelId)
        {
            ChannelId = channelId;
        }

        public override bool CanExecuteInContext(IMessage message, IServiceProvider serviceProvider)
        {
            return message.Channel.Id == ChannelId;
        }
    }
}
