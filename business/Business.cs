using Models;
using persistentie;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace business
{
    public class Business
    {
        List<subjectItem> subjectItems;
        List<Subranking> subRankings;
        
        //we start by creating subrankings of 2, then we combine them to 4, 8, 16, 32, ..., we make sure that there are no groups that stay behind in smaller sizes
        enum State { SRankCreating, Merging, finished}
        State state;
        //for SrankCreating state:
        int subjectItemsIterator;


        //for merging state
        Subranking creatieRuimte;
        Subranking MergeSource1;
        Subranking MergeSource2;
        int Source1Iterator;
        int Source2Iterator; 
        int subRankingIndex;    //current index in the subranking, the sources are based on this one.
        DirectComparator CurrentComparison;


        PersistenceObject opslag { get; set; }
        int subjectId { get; set; }
        Random r { get; set; }
        public Business(int subjectId, PersistenceObject persistence)
        {
            opslag = persistence;
            subjectId = subjectId;
            r = new Random();
            subjectItems = new List<subjectItem>();
            for (int i = 0; i < 15; i++)
            {
                subjectItem item = new subjectItem { Id = (i/2), Image = "", Text = new String[] { (i/2).ToString() }, SubjectId = subjectId };
                subjectItems.Add(item);
            }
            for (int i = 0; i < 15; i++)//sjuffel array
            {
                int rand = r.Next(14);
                subjectItem temp = subjectItems[rand];
                subjectItems[rand] = subjectItems[i];
                subjectItems[i] = temp;
            }
            if(subjectItems.Count <= 1)
            {
                throw new Exception() { Source = "Er moet meer dan 1 item zijn" };
            }
            state = State.SRankCreating;
            subjectItemsIterator = 0;
            subRankings = new List<Subranking>();
        }


        public subjectItem[] Give_options()
        {
            if (state == State.SRankCreating) //the switch no next state happens in the Give_result code
            {
                if(subjectItemsIterator < subjectItems.Count - 1)
                {
                    return new subjectItem[2] { subjectItems[subjectItemsIterator], subjectItems[subjectItemsIterator + 1] };
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else if (state == State.Merging)
            {
                return CurrentComparison.GiveOptions();
            }
            else if (state == State.finished) {
                return null;
            }
            return null;
        }

        public void Give_result(subjectItem[] ranked, bool tied)    //index 0 = winner, index 1 = loser
        {
            if (state == State.SRankCreating)
            {
                Subranking subranking = new Subranking();
                if (tied)
                {
                    RankingPlace rp = new RankingPlace();
                    rp.itemsThisRanking.Add(ranked[0]);
                    rp.itemsThisRanking.Add(ranked[1]);
                    subranking.rankedHighToLow.Add(rp);
                }
                else
                {
                    RankingPlace rp1 = new RankingPlace();
                    rp1.itemsThisRanking.Add(ranked[0]);
                    RankingPlace rp2 = new RankingPlace();
                    rp2.itemsThisRanking.Add(ranked[1]);
                    subranking.rankedHighToLow.Add(rp1);
                    subranking.rankedHighToLow.Add(rp2);
                }
                //update needed data so the next query can be asked
                subjectItemsIterator += 2;
                subRankings.Add(subranking);
                subranking = new Subranking();
                if(subjectItemsIterator >= subjectItems.Count - 1)
                {
                    if (subjectItemsIterator == subjectItems.Count)
                    {  //we iterated thourgh everything

                    }
                    if (subjectItemsIterator == subjectItems.Count -1)
                    {  //there is a single subjectitem left, we make this one its onwn subject ranking before continuing
                        RankingPlace rp = new RankingPlace();
                        rp.itemsThisRanking.Add(subjectItems[subjectItemsIterator]);
                        subranking.rankedHighToLow.Add(rp);
                        subRankings.Add(subranking);
                    }
                    state = State.Merging;
                    subRankingIndex = 0;
                    PrepareMergingStep();
                }
            }
            else if (state == State.Merging)
            {
                CurrentComparison.propagateWinner(ranked[0]);
                
                if (CurrentComparison.done)
                {
                    creatieRuimte.rankedHighToLow.AddRange(CurrentComparison.RankedReturn().Take(CurrentComparison.RankedReturn().Count-1));
                    if(CurrentComparison.LosingStack() == 2)
                    {
                        Source1Iterator++;
                        MergeSource2.rankedHighToLow[Source2Iterator] = CurrentComparison.RankedReturn().TakeLast(1).ToArray()[0];   //in case of tied list reduction
                        if (Source1Iterator == MergeSource1.rankedHighToLow.Count)//no need to ask anymore for this merge, we know the result
                        {
                            creatieRuimte.rankedHighToLow.AddRange(MergeSource2.rankedHighToLow.Skip(Source2Iterator));
                            FinishMergingStepAndNew();
                        }
                        else
                        {
                            CurrentComparison = new DirectComparator(MergeSource1.rankedHighToLow[Source1Iterator], MergeSource2.rankedHighToLow[Source2Iterator]);
                        }
                    }
                    else
                    {
                        Source2Iterator++;
                        MergeSource1.rankedHighToLow[Source1Iterator] = CurrentComparison.RankedReturn().TakeLast(1).ToArray()[0];
                        if (Source2Iterator == MergeSource2.rankedHighToLow.Count)//no need to ask anymore for this merge, we know the result
                        {
                            creatieRuimte.rankedHighToLow.AddRange(MergeSource1.rankedHighToLow.Skip(Source1Iterator));
                            FinishMergingStepAndNew();
                        }
                        else
                        {
                            CurrentComparison = new DirectComparator(MergeSource1.rankedHighToLow[Source1Iterator], MergeSource2.rankedHighToLow[Source2Iterator]);
                        }
                    }
                }
            }
            else if (state == State.finished)
            {
            }
        }
        void FinishMergingStepAndNew()
        {
            subRankings.Add(creatieRuimte);
            subRankings.Remove(MergeSource1);
            subRankings.Remove(MergeSource2);
            PrepareMergingStep();
        }
        void PrepareMergingStep()
        {
            if(subRankings.Count == 1)
            {
                state = State.finished;
                return;
            }
            creatieRuimte = new Subranking();
            Source1Iterator = 0;
            Source2Iterator = 0;
            MergeSource1 = subRankings[subRankingIndex % subRankings.Count];
            MergeSource2 = subRankings[(subRankingIndex+1)% subRankings.Count];
            CurrentComparison = new DirectComparator(MergeSource1.rankedHighToLow[Source1Iterator], MergeSource2.rankedHighToLow[Source2Iterator]);
        }

        public List<RankingItem> GetFinalRankedList()
        {
            if(state != State.finished)
            {
                return new List<RankingItem>();
            }
            //everything should be in subrankings[0]
            var list = new List<RankingItem>();
            for ( int i = 0; i < subRankings[0].rankedHighToLow.Count; i++ ) {
                foreach (subjectItem si in subRankings[0].rankedHighToLow[i].itemsThisRanking)
                {
                    RankingItem RA = new RankingItem();
                    RA.subjectitem = si;
                    RA.Rank = i;
                    list.Add(RA);
                }
            }
            return list;
        }

        public ComparedRankingResult[] Compare()
        {
            return new ComparedRankingResult[0];
        }

        public void SaveCurrent(string userName)
        {

        }

        public List<Subject> Give_all_subjects()
        {
            return null;
        }
    }
}
