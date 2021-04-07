using System;
using System.Collections.Generic;

namespace RocketLeagueScrimFinder.Data
{
    public class Attributes
    {
        public int PlaylistId { get; set; }
        public int Season { get; set; }
    }

    public class Metadata
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }
    }

    public class Tier
    {
        public object Rank { get; set; }
        public object Percentile { get; set; }
        public string DisplayName { get; set; }
        public string DisplayCategory { get; set; }
        public string Category { get; set; }
        public Metadata Metadata { get; set; }
        public int Value { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayType { get; set; }
    }

    public class Division
    {
        public object Rank { get; set; }
        public object Percentile { get; set; }
        public string DisplayName { get; set; }
        public string DisplayCategory { get; set; }
        public string Category { get; set; }
        public Metadata Metadata { get; set; }
        public int Value { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayType { get; set; }
    }

    public class MatchesPlayed
    {
        public object Rank { get; set; }
        public object Percentile { get; set; }
        public string DisplayName { get; set; }
        public string DisplayCategory { get; set; }
        public string Category { get; set; }
        public Metadata Metadata { get; set; }
        public int Value { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayType { get; set; }
    }

    public class WinStreak
    {
        public object Rank { get; set; }
        public object Percentile { get; set; }
        public string DisplayName { get; set; }
        public string DisplayCategory { get; set; }
        public string Category { get; set; }
        public Metadata Metadata { get; set; }
        public int Value { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayType { get; set; }
    }

    public class Rating
    {
        public object Rank { get; set; }
        public object Percentile { get; set; }
        public string DisplayName { get; set; }
        public string DisplayCategory { get; set; }
        public string Category { get; set; }
        public Metadata Metadata { get; set; }
        public int Value { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayType { get; set; }
    }

    public class Stats
    {
        public Tier Tier { get; set; }
        public Division Division { get; set; }
        public MatchesPlayed MatchesPlayed { get; set; }
        public WinStreak WinStreak { get; set; }
        public Rating Rating { get; set; }
    }

    public class Datum
    {
        public string Type { get; set; }
        public Attributes Attributes { get; set; }
        public Metadata Metadata { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Stats Stats { get; set; }
    }

    public class TrackerData
    {
        public List<Datum> Data { get; set; }
    }
}
