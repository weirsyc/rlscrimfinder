using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace RocketLeagueScrimFinder.Models
{
    public class ScrimFinderContext : DbContext
    {
        public ScrimFinderContext(DbContextOptions<ScrimFinderContext> options) : base(options)
        {
        }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ScrimEvent> ScrimEvents { get; set; }
        public DbSet<ScrimRequest> ScrimRequests { get; set; }
        public DbSet<LobbyInfo> LobbyInfos { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
    }
}
