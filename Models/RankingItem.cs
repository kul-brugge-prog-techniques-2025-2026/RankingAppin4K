using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class RankingItem
    {
        public int Id { get; set; }
        public int subjectItemId {  get; set; }

        public int RankingResultId { get; set; }
        public int Rank { get; set; }

        [JsonIgnore]//mogelijk compositie opslaan zonder in json te steken
        public subjectItem subjectitem { get; set; }


    }
}
