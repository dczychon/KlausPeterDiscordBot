using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Attributes;
using DiscordBot.Util.Logging;
using Newtonsoft.Json.Linq;
using NLog;

namespace KlausPeterBot.BotCommands
{
    [CommandVerb("fortnite-stats", Description = "Display fortnite battle-royale stats")]
    public sealed class FortniteStatsCommand : ICommand
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly HttpClient _httpClient;
        private readonly KlausPeterBotConfig _botConfig;

        [CommandArgument(1, nameof(Platform), DefaultValue = "pc", HelpText = "Platform for the user. Can be pc, xb1 or psn")]
        public string Platform { get; set; }

        [CommandArgument(0, nameof(EpicUsername), Required = true, HelpText = "The epic username")]
        public string EpicUsername { get; set; }

        public FortniteStatsCommand(HttpClient httpClient, KlausPeterBotConfig botConfig)
        {
            _httpClient = httpClient;
            _botConfig = botConfig;
        }

        public async Task Execute(IMessage rawMessage)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.fortnitetracker.com/v1/profile/{Platform}/{EpicUsername}");
            request.Headers.Add("TRN-Api-Key", _botConfig.FortniteTrackerToken);

            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(3);

                HttpResponseMessage response;
                using (new EntryExitLog(_logger.Debug, "Calling fortnite tracker api " + request.RequestUri))
                {
                    response = await _httpClient.SendAsync(request);
                }

                if (response.IsSuccessStatusCode)
                {
                    _logger.Debug("Fortnite tracker api call successful with code " + Convert.ToInt32(response.StatusCode));
                    try
                    {
                        string matchesPlayed = string.Empty;
                        string wins = string.Empty;
                        string kills = string.Empty;
                        string kd = string.Empty;

                        JObject statsJson = JObject.Parse(await response.Content.ReadAsStringAsync());

                        foreach (JToken child in statsJson["lifeTimeStats"].Children())
                        {
                            switch (child["key"].ToString())
                            {
                                case "Matches Played":
                                    matchesPlayed = child["value"].ToString();
                                    break;

                                case "Wins":
                                    wins = child["value"].ToString();
                                    break;

                                case "Kills":
                                    kills = child["value"].ToString();
                                    break;

                                case "K/d":
                                    kd = child["value"].ToString();
                                    break;
                            }
                        }

                        await rawMessage.Channel.SendMessageAsync($"Fortnite stats for **{statsJson["epicUserHandle"]}** on **{statsJson["platformNameLong"]}**:\n*Matches played*: {matchesPlayed}\n*Wins*: {wins}\n*Kills*: {kills}\n*K/D*: {kd}");
                    }
                    catch (Exception parsingEx)
                    {
                        _logger.Error(parsingEx, "Error while parsing fortnite tracker api response");
                        await rawMessage.Channel.SendMessageAsync("Fortnite api has send some shit that i can´t understand :ghost::persevere:");
                    }
                }
                else
                {
                    _logger.Warn($"Fortnite tracker api returned {Convert.ToInt32(response.StatusCode)} status code");
                    await rawMessage.Channel.SendMessageAsync("Couldn´t receive stats. Check username and platform and try again :shrug:");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during request to fortnite tracker api");
                await rawMessage.Channel.SendMessageAsync("Couldn´t receive stats. Check username and platform and try again :shrug:");
            }
        }
    }
}
