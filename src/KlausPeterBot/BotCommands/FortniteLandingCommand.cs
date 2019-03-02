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
    [CommandVerb("fortnite-drop", Description = "Selects a random position on the fortnite map")]
    public class FortniteLandingCommand : ICommand
    {
        private static readonly string[] _pois = {"Junk Junction", "Lazy Links", "Haunted Hills", "Pleasant Park", "Tomato Temple", "Wailing Woods", "Snobby Shores", "Dusty Divot", "Retail Row", "Lonely Lodge", "Salty Springs", "Tilted Towers", "Shifty Shafts", "Fatal Fields", "Paradise Palms", "Flush Factory", "Lucky Landing", "The Block", "Polar Peak", "Frosty Flights", "Happy Hamlet", "Loot Lake"};

        private readonly Random _rnd = new Random();

        [CommandArgument(0, nameof(Type), DefaultValue = "coordinates", HelpText = "Defines the output. 'coordinates' to return raw map coordinates or 'area' for area names")]
        public string Type { get; set; }
        public async Task Execute(IMessage rawMessage)
        {
            string loc = string.Empty;

            switch (Type.ToLower())
            {
                case "coordinates":
                    loc = GetRandomCoordinates();
                    break;

                case "area":
                    loc = GetRandomLocation();
                    break;

                default:
                    await rawMessage.Channel.SendMessageAsync($"\"{Type}\" is a unknown value for argument {nameof(Type)}");
                    return;
            }


            await rawMessage.Channel.SendMessageAsync($"The fortnite god wants you to drop at **{loc}** :angel:");
        }

        private string GetRandomCoordinates()
        {
            return $"{Convert.ToChar(_rnd.Next(1, 11) + 64)}{_rnd.Next(1, 11)}";
        }

        private string GetRandomLocation()
        {
            return _pois[_rnd.Next(0, _pois.Length - 1)];
        }
    }
}
