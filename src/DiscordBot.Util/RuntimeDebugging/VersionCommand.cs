using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Attributes;

namespace DiscordBot.Util.RuntimeDebugging
{
    [RestrictToBotAdmin]
    [CommandVerb("info", Description = "Displays the bots version number", Hidden = true)]
    internal sealed class InfoCommand : ICommand
    {
        public async Task Execute(IMessage rawMessage)
        {
            await rawMessage.Channel.SendMessageAsync($"System name:\t{Environment.MachineName}\n" +
                $"Bot version:\t{Assembly.GetEntryAssembly().GetName().Version}\n" +
                $".NET Core Runtime:\t{Path.GetFileName(Path.GetDirectoryName(RuntimeEnvironment.GetRuntimeDirectory()))}\n" +
                $"Bot started (UTC):\t{BotProgram.BotStartupTimeUtc.ToString("dd.MM.yyyy HH:mm:ss")}\n" +
                $"Last Discord connection lost (UTC):\t{BotProgram.LastDiscordConnectionLostUtc?.ToString("dd.MM.yyyy HH:mm:ss") ?? "never"}");
        }

        private static Version GetBotVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        private static string GetDotNetRuntimeVersion()
        {
            return Path.GetFileName(Path.GetDirectoryName(RuntimeEnvironment.GetRuntimeDirectory()));   //Does this also work if we are a self contained application??
        }
    }
}
