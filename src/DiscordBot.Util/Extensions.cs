using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Util
{
    public static class Extensions
    {
        internal static bool HasIndex(this Array array, int index)
        {
            return array.Length > index;
        }

        internal static string ToNullString(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? "null" : str;
        }

        public static bool IsPrivateMessage(this IMessage message)
        {
            return message.Channel.Name == $"@{message.Author.Username}#{message.Author.Discriminator}";
        }
    }
}
