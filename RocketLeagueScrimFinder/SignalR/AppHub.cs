using Microsoft.AspNetCore.SignalR;
using RocketLeagueScrimFinder.Extensions;
using System;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.SignalR
{
    public class AppHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(Context.User);
                UserConnections.Add(steamId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(Context.User);
                UserConnections.Remove(steamId, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(ex);
        }
    }
}
