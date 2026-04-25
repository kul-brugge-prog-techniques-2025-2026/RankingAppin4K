using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace business
{
    internal class Subranking
    {
        public RankingPlace[] rankedHighToLow;
        public int getSize()
        {
            int num = 0;
            for (int i = 0; i < rankedHighToLow.Length; i++) { 
                num += rankedHighToLow[i].itemsThisRanking.Length
            }
            return num;
        }
    }
}
