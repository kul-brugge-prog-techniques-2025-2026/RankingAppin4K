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
        int lto_ListIndex;//0=a,1=b
        int lto_SingleIndex;
        RankingPlace lto_Ahead;
        RankingPlace lto_After;
        RankingPlace lto_Ties;      //for that one item and those who tie with it


        RankingPlace ltl_Winners;   //for those who only won
        RankingPlace ltl_Losers;    //for those who only lost
        RankingPlace ltl_Ties;      //for those who tied or did not only win or lose. guaranteed for no overlap if we ask once for the biggest group.
        int ltl_BigLIndex;
        int ltl_smallLIndex;
        int ltl_BLIterator;
        int ltl_SLIterator;
        int[] ltl_sizes;    //0 = biggest, 1 = smallest
        bool[] ltl_ABloserentered;

        public DirectComparator(RankingPlace a, RankingPlace b)
        {
            done = false;
            RankedResults = new List<RankingPlace>();
            ab = new RankingPlace[2] { a, b };
            if (ab[0].itemsThisRanking.Count == 1 && ab[1].itemsThisRanking.Count == 1)
            {
                modest = mode.normal;
            }
            else if (ab[0].itemsThisRanking.Count > 1 && ab[1].itemsThisRanking.Count > 1)
            {
                modest = mode.listtolist;
                ltl_Winners = new RankingPlace();
                ltl_Losers = new RankingPlace();
                ltl_Ties = new RankingPlace();
                ltl_BLIterator = 0;
                ltl_SLIterator = 0;
                if (ab[0].itemsThisRanking.Count > ab[1].itemsThisRanking.Count)//okay for same size
                {
                    ltl_BigLIndex = 0;
                    ltl_smallLIndex = 1;
                }
                else
                {
                    ltl_BigLIndex = 1;
                    ltl_smallLIndex = 0;
                }
                ltl_sizes = new int[] { ab[ltl_BigLIndex].itemsThisRanking.Count, ab[ltl_smallLIndex].itemsThisRanking.Count };
                ltl_ABloserentered = new bool[] { false, false};
            }
            else //list to one code
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
                lto_Ties = new RankingPlace();
                lto_Ties.itemsThisRanking.Add(ab[lto_SingleIndex].itemsThisRanking[0]);
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
                return new subjectItem[2] { ab[ltl_BigLIndex].itemsThisRanking[ltl_BLIterator], ab[ltl_smallLIndex].itemsThisRanking[ltl_SLIterator] };
            }
        }
        public void propagateWinner(subjectItem winner, bool tied)
        {
            if (modest == mode.normal)
            {
                done = true;
                if (tied)
                {
                    RankingPlace rp = new RankingPlace();
                    rp.itemsThisRanking.Add(ab[0].itemsThisRanking[0]);
                    rp.itemsThisRanking.Add(ab[1].itemsThisRanking[0]);
                    RankedResults.Add(rp);
                    losingStack = 0;
                } else
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
                if (tied)
                {
                    lto_Ties.itemsThisRanking.Add(ab[lto_ListIndex].itemsThisRanking[lto_Iterator]);
                } else
                if (winner == ab[lto_ListIndex].itemsThisRanking[lto_Iterator])
                {
                    lto_Ahead.itemsThisRanking.Add(ab[lto_ListIndex].itemsThisRanking[lto_Iterator]);
                }
                else
                {
                    lto_After.itemsThisRanking.Add(ab[lto_ListIndex].itemsThisRanking[lto_Iterator]);
                }
                lto_Iterator++;
                if (lto_Iterator == ab[lto_ListIndex].itemsThisRanking.Count)
                {
                    done = true;
                    if (lto_Ahead.itemsThisRanking.Count > 0)
                    {
                        RankedResults.Add(lto_Ahead);
                    }
                    RankedResults.Add(lto_Ties);
                    if (lto_After.itemsThisRanking.Count > 0) {
                        RankedResults.Add(lto_After);
                    }
                }
            }
            else if (modest == mode.listtolist)
            {
                var smalllistitem = ab[ltl_smallLIndex].itemsThisRanking[ltl_SLIterator];
                var biglistitem = ab[ltl_BigLIndex].itemsThisRanking[ltl_BLIterator];
                if (tied)
                {
                    ltl_Ties.itemsThisRanking.Add(biglistitem);
                    ltl_Ties.itemsThisRanking.Remove(smalllistitem);
                    ltl_Losers.itemsThisRanking.Remove(smalllistitem);
                    ltl_Winners.itemsThisRanking.Remove(smalllistitem);
                    ltl_Ties.itemsThisRanking.Add(smalllistitem);
                }
                else if(winner == biglistitem)    //big list winner
                {
                    ltl_Winners.itemsThisRanking.Add(biglistitem);
                    ltl_ABloserentered[ltl_smallLIndex] = true;
                    if (ltl_Winners.itemsThisRanking.Contains(smalllistitem))
                    {
                        ltl_Winners.itemsThisRanking.Remove(smalllistitem);
                        ltl_Ties.itemsThisRanking.Add(smalllistitem);
                    }
                    else if (ltl_Ties.itemsThisRanking.Contains(smalllistitem))
                    {
                        //good
                    }
                    else    //if loser is in the loser list or in no list
                    {
                        ltl_Losers.itemsThisRanking.Remove(smalllistitem);
                        ltl_Losers.itemsThisRanking.Add(smalllistitem);
                    }
                }
                else        //small list winner
                {
                    ltl_Losers.itemsThisRanking.Add(biglistitem);
                    ltl_ABloserentered[ltl_BigLIndex] = true;
                    if (ltl_Losers.itemsThisRanking.Contains(smalllistitem))
                    {
                        ltl_Losers.itemsThisRanking.Remove(smalllistitem);
                        ltl_Ties.itemsThisRanking.Add(smalllistitem);
                    }
                    else if (ltl_Ties.itemsThisRanking.Contains(smalllistitem))
                    {
                        //good
                    }
                    else    //if winner is in the winner list or in no list
                    {
                        ltl_Winners.itemsThisRanking.Remove(smalllistitem);
                        ltl_Winners.itemsThisRanking.Add(smalllistitem);
                    }
                }
                ltl_BLIterator++;
                ltl_SLIterator = (ltl_SLIterator + 1) % ltl_sizes[1];
                if(ltl_BLIterator == ltl_sizes[0])
                {
                    done = true;
                    if (ltl_Winners.itemsThisRanking.Count > 0) { RankedResults.Add(ltl_Winners); }
                    if (ltl_Ties.itemsThisRanking.Count > 0) { RankedResults.Add(ltl_Ties); }
                    if (ltl_Losers.itemsThisRanking.Count > 0) { RankedResults.Add(ltl_Losers); }
                }
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
                if (lto_After.itemsThisRanking.Count == 0)
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
                if (ltl_ABloserentered[0] == false)
                {
                    return 2;
                }
                return 1;
            }
        }
        public bool reapearing()    //when the lowest scoring group needs to return to its stack for comparison against the next item in the enemy stack
        {
            if(modest == mode.listtolist)
            {
                if (ltl_ABloserentered[0] && ltl_ABloserentered[1]) {
                    return false;
                }
            }else if(modest == mode.normal)
            {
                if (losingStack == 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
