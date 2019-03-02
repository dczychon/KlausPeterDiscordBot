using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Util
{
    public class BotConfiguration
    {
        /// <summary>
        /// Discord user ids of the bot admins
        /// </summary>
        public IEnumerable<ulong> BotAdministrators { get; set; } = Array.Empty<ulong>();

        /// <summary>
        /// If <see cref="true"/> some commands for debug purposes are added during initialization
        /// </summary>
        public bool EnableDebugCommandsForAdmins { get; set; } = true;

        /// <summary>
        /// If <see cref="true"/> the Bot only responds to bot administrators in private DM channnels
        /// </summary>
        public bool RespondeOnlyToAdminsInPrivateChannels { get; set; } = false;

        /// <summary>
        /// Initializes a new instace of <see cref="BotConfiguration"/> with default values
        /// </summary>
        public BotConfiguration()
        {

        }
    }
}
