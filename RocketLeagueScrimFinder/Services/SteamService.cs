using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketLeagueScrimFinder.Services
{
    public class SteamService
    {
        private IMemoryCache _cache;
        private IConfiguration _configuration;
        public SteamService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _cache = memoryCache;
            _configuration = configuration;
        }
        internal List<KeyValue> GetSteamInfo(string steamId)
        {
            if (_cache.TryGetValue($"{steamId}_steam", out List<KeyValue> cachedInfo))
            {
                return cachedInfo;
            }
            try
            {
                using (var steamUser = WebAPI.GetInterface("ISteamUser", _configuration.GetValue<string>("SteamApiKey")))
                {
                    var args = new Dictionary<string, object>();
                    args.Add("steamids", steamId);
                    var results = steamUser.Call("GetPlayerSummaries", 2, args);
                    var userInfo = results.Children.ToList().FirstOrDefault().Children.FirstOrDefault().Children;
                    _cache.Set($"{steamId}_steam", userInfo, DateTime.Now.AddMinutes(60));
                    return userInfo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get tracker data: {0}", ex);
            }
            return null;
        }

        internal string GetSteamDisplayName(string steamId)
        {
            return this.GetSteamInfo(steamId).FirstOrDefault(u => u.Name == "personaname")?.Value;
        }
    }
}
