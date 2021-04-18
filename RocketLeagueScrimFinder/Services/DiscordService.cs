using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using RocketLeagueScrimFinder.Data;
using RocketLeagueScrimFinder.Models;
using System;
using System.Linq;

namespace RocketLeagueScrimFinder.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private IConfiguration _configuration;

        public DiscordService(IConfiguration configuration)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Debug });
            _configuration = configuration;
            this.SetupClient();
        }

        private async void SetupClient()
        {
            await _client.LoginAsync(TokenType.Bot, _configuration.GetValue<string>("DiscordBotToken"));
            await _client.StartAsync();
        }

        private async void SendPrivateMessage(string discordId, string message)
        {
            try
            {
                var userSplit = discordId.Split('#');
                var user = _client.GetUser(userSplit[0], userSplit[1]);
                if (user != null)
                {
                    await UserExtensions.SendMessageAsync(user, message);
                }
            }
            catch (Exception)
            {
                //TODO logging
            }
        }

        internal void SendMessage(DmType dmType, string discordId, UserInfo opponent = null)
        {
            var messageText = FormatDm(dmType, opponent);
            if (messageText != null)
            {
                SendPrivateMessage(discordId, messageText);
            }
        }

        private string FormatDm(DmType dmType, UserInfo opponent = null)
        {
            switch (dmType)
            {
                case DmType.MatchFound:
                    return $"Match found! Opponent: {opponent.DisplayName} (https://steamcommunity.com/profiles/{opponent.SteamId}) - {opponent.Mmr}. Please respond by going to https://rlscrimfinder.com/Matchmaking";
                case DmType.MatchAccepted:
                    return $"Match accepted! Please join the game lobby: https://rlscrimfinder.com/Matchmaking";
                case DmType.ScheduleAccept:
                    return $"Scheduled match was accepted by {opponent.DisplayName} (https://steamcommunity.com/profiles/{opponent.SteamId}) - {opponent.Mmr}. You can join the lobby by going to https://rlscrimfinder.com/Schedule";
                case DmType.ScheduleRequest:
                    return $"You have a new match request from {opponent.DisplayName} (https://steamcommunity.com/profiles/{opponent.SteamId}) - {opponent.Mmr}. Please respond by going to https://rlscrimfinder.com/Schedule";
            }
            return null;
        }
    }
}
