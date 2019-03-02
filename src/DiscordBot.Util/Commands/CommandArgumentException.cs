using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Util.Commands
{
    /// <summary>
    /// Exception that gets thrown on command argument errors
    /// </summary>
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException(string message) : base(message)
        {
            
        }
    }
}
