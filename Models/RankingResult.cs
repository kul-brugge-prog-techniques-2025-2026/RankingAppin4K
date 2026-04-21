using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class RankingResult
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public required string Name { get; set; }
    }

    public class RankedItemDisplay
    {
        public int Rank { get; set; }
        public required string Name { get; set; }
        public required string Image { get; set; }
    }

    public class ComparedRankingResult : RankingResult
    {
        public List<RankedItemDisplay> RankedItems { get; set; } = new List<RankedItemDisplay> ();
    }
}
