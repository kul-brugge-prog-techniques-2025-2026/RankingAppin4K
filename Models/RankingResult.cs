using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class RankingResult
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public required string Name { get; set; }
        [JsonIgnore]
        public List<RankingItem> RankedItems { get; set; } = new List<RankingItem>();
    }

    public class RankedItemDisplay
    {
        public int Rank { get; set; }
        public required string Name { get; set; }
        public required string Image { get; set; }
    }

    public class ComparedRankingResult : RankingResult
    {
        public double SimilarityRate { get; set; }
    }
}
