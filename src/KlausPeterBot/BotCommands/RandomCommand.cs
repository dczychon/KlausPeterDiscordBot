using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Attributes;

namespace KlausPeterBot.BotCommands
{
    [CommandVerb("random", Description = "Selects a random value")]
    public class RandomCommand : ICommand
    {
        [CommandArgument(0, nameof(Values), Required = true, HelpText = "A comma separated list with values. For example: bike,phone,house")]
        public string Values { get; set; }

        public async Task Execute(IMessage rawMessage)
        {
            string[] val = Values.Split(',');
            Random rnd = new Random();

            string winner = val[rnd.Next(0, val.Length - 1)];

            await rawMessage.Channel.SendMessageAsync($"**{winner}** won the random contest out of *{val.Length}* possibilities :tada:");
        }
    }
}
