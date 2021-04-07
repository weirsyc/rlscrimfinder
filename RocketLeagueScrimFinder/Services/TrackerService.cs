using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RocketLeagueScrimFinder.Data;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.Services
{
    public class TrackerService
    {
        private IMemoryCache _cache;
        private IConfiguration _configuration;
        public TrackerService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _cache = memoryCache;
            _configuration = configuration;
        }
        public async Task<int> GetMmr(string userId)
        {
            if (_cache.TryGetValue($"{userId}_mmr", out var cachedMmr))
            {
                return (int)cachedMmr;
            }
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri($"https://public-api.tracker.gg/v2/rocket-league/standard/profile/steam/{userId}/segments/playlist?season=16"), //Season 2
                        Headers = {
                        { "TRN-Api-Key", _configuration.GetValue<string>("TrnApiKey") }
                    }
                    };
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var trackerData = JsonConvert.DeserializeObject<TrackerData>(result);
                        var threesData = trackerData.Data.FirstOrDefault(t => t.Attributes.PlaylistId == 13); //Standard 3s
                        var threesMmr = threesData?.Stats?.Rating?.Value ?? 0;
                        _cache.Set($"{userId}_mmr", threesMmr, DateTime.Now.AddMinutes(60));
                        return threesMmr;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to get tracker data: {0}", ex);
            }
            return 0;
        }
    }
}
