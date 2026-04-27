using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace business
{
    internal class DirectComparator
    {
        RankingPlace[] ab;    //because we can end up comparing multiple tied groups, we need to be able to handle that
        //This is an object that gets created for every comparison between Subranking items (which can be groups of tied objects)
        //this object handles the suply of question objects for the user and return of a ranked subranking items (the last of the at least 2 wil need to be pushed back to the stack again.
        
        enum mode { normal, listtoone, listtolist };//listtoone = one item is a tied list, listtolist = they are both lists
        mode modest;
        public bool done { get; set; }
        List<RankingPlace> RankedResults;
        int losingStack;

        int lto_Iterator;
        int lto_ListIndex;
        int lto_SingleIndex;
        RankingPlace lto_Ahead;
        RankingPlace lto_After;

        RankingPlace ltl_Winners;   //for those who only won
        RankingPlace ltl_Losers;    //for those who only lost
        RankingPlace ltl_Ties;      //for those who tied or did not only win or lose. guaranteed for no overlap if we ask once for the biggest group.
        int ltl_BigLIndex;
        int ltl_sameSize;
        int ltl_smallLIndex;

        public DirectComparator(RankingPlace a, RankingPlace b)
        {
            done = false;
            RankedResults = new List<RankingPlace>();
            ab = new RankingPlace[2] { a, b };
            if (ab[0].itemsThisRanking.Count == 1 && ab[1].itemsThisRanking.Count == 1)
            {
                modest = mode.normal;
            }
            else if(ab[0].itemsThisRanking.Count > 1 && ab[1].itemsThisRanking.Count > 1)
            {
                modest = mode.listtolist;
                ltl_Winners = new RankingPlace();
                ltl_Losers = new RankingPlace();
                ltl_Ties = new RankingPlace();
                ltl_BigLIndex;
                ltl_sameSize;
                ltl_smallLIndex;
            }
            else
            {
                modest = mode.listtoone;
                if (ab[0].itemsThisRanking.Count > 1)
                {
                    lto_ListIndex = 0;
                }
                else {
                    lto_ListIndex = 1;
                }
                lto_SingleIndex = Math.Abs(lto_ListIndex - 1);//0->1 en 1->0
                lto_Iterator = 0;
                lto_After = new RankingPlace();
                lto_Ahead = new RankingPlace();
            }
        }
        
        public subjectItem[] GiveOptions()
        {
            if (modest == mode.normal)
            {
                return new subjectItem[2] { ab[0].itemsThisRanking[0], ab[1].itemsThisRanking[0] };
            }
            else if (modest == mode.listtoone)
            {
                return new subjectItem[2] { ab[lto_ListIndex].itemsThisRanking[lto_Iterator], ab[lto_SingleIndex].itemsThisRanking[0] };
            }
            else {
                //Ask once for every item from the biggest group, then we have: the ones who won, the ones who lost and the ones who tied, only if the loser group is from a single source then we add them back to the og 2 comparison stacks
                
                return new subjectItem[2] { ab[0].itemsThisRanking[0], ab[1].itemsThisRanking[0] };
            }
        }
        public void propagateWinner(subjectItem winner)
        {
            if (modest == mode.normal)
            {
                done = true;
                if (ab[0].itemsThisRanking[0] == winner)
                {
                    RankedResults.Add(ab[0]);
                    RankedResults.Add(ab[1]);
                    losingStack = 2;
                }
                else
                {
                    RankedResults.Add(ab[1]);
                    RankedResults.Add(ab[0]);
                    losingStack = 1;
                }
            }
            else if (modest == mode.listtoone)
            {
                if(winner == ab[lto_ListIndex].itemsThisRanking[lto_Iterator])
                {
                    lto_Ahead.itemsThisRanking.Add(ab[lto_ListIndex].itemsThisRanking[lto_Iterator]);
                }
                else
                {
                    lto_Ahead.itemsThisRanking.Add(ab[lto_ListIndex].itemsThisRanking[lto_Iterator]);
                }
                lto_Iterator++;
                if (lto_Iterator == ab[lto_ListIndex].itemsThisRanking.Count)
                {
                    done = true;
                }
            }
            else if (modest == mode.listtolist)
            {

            }
        }

        public List<RankingPlace> RankedReturn()
        {
            return RankedResults;
        }
        public int LosingStack()
        {//1 for A, 2 for B
            if (modest == mode.normal)
            {
                return losingStack;
            }
            else if (modest == mode.listtoone)
            {
                if(lto_After.itemsThisRanking.Count == 0)
                {
                    return lto_SingleIndex + 1;
                }
                else
                {
                    return lto_ListIndex + 1;
                }
            }
            else
            {
                return 0;
            }
        }

    }
}
