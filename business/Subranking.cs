using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace business
{
    internal class Subranking
    {
        public Subranking() {
            rankedHighToLow = new List<RankingPlace>();
        } 
        public List<RankingPlace> rankedHighToLow;
        public int getSize()
        {
            int num = 0;
            for (int i = 0; i < rankedHighToLow.Count; i++) {
                num += rankedHighToLow[i].itemsThisRanking.Count;
            }
            return num;
        }
    }
}
