using Models;
using persistentie;
using System;
using System.Collections.Generic;
using System.Text;

namespace business
{
    public class Business
    {
        subjectItem[] subjectItems;
        Subranking[] subRankings;
        
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


        PersistenceObject opslag { get; set; }
        int subjectId { get; set; }
        Random r { get; set; }
        public Business(int subjectId, PersistenceObject persistence)
        {
            opslag = persistence;
            subjectId = subjectId;
            r = new Random();
            for (int i = 0; i < 15; i++)
            {
                subjectItem item = new subjectItem { Id = i, Image = "", Text = new String[] { i.ToString() }, SubjectId = subjectId };
                subjectItems.Append(item);
            }
            for (int i = 0; i < 15; i++)//sjuffel array
            {
                int rand = r.Next(14);
                subjectItem temp = subjectItems[rand];
                subjectItems[rand] = subjectItems[i];
                subjectItems[i] = temp;
            }
            if(subjectItems.Length >= 1)
            {
                throw new Exception() { Source = "Er moet meer dan 1 item zijn" };
            }
            state = State.SRankCreating;
            subjectItemsIterator = 0;
            subRankings = Array.Empty<Subranking>();
        }


        public subjectItem[] Give_options()
        {
            if (state == State.SRankCreating) //the switch no next state happens in the Give_result code
            {
                if(subjectItemsIterator < subjectItems.Length - 1)
                {
                    return new subjectItem[2] { subjectItems[subjectItemsIterator], subjectItems[subjectItemsIterator + 1] };
                }
                else
                {
                    state = State.Merging;  //not supposed to happen
                    return Give_options();
                }
            }
            else if (state == State.Merging)
            {

            }
            else if (state == State.finished) {
                return null;
            }
            return new subjectItem[2];
        }

        public void Give_result(subjectItem[] ranked, bool tied)    //index 0 = winner, index 1 = loser
        {
            if (state == State.SRankCreating)
            {
                Subranking subranking = new Subranking();
                if (tied)
                {
                    RankingPlace rp = new RankingPlace();
                    rp.itemsThisRanking.Append(ranked[0]);
                    rp.itemsThisRanking.Append(ranked[1]);
                    subranking.rankedHighToLow.Append(rp);
                }
                else
                {
                    RankingPlace rp1 = new RankingPlace();
                    rp1.itemsThisRanking.Append(ranked[0]);
                    RankingPlace rp2 = new RankingPlace();
                    rp2.itemsThisRanking.Append(ranked[1]);
                    subranking.rankedHighToLow.Append(rp1);
                    subranking.rankedHighToLow.Append(rp2);
                }
                //update needed data so the next query can be asked
                subjectItemsIterator += 2;
                subRankings.Append(subranking);
                subranking = new Subranking();
                if(subjectItemsIterator >= subjectItems.Length - 1)
                {
                    if (subjectItemsIterator == subjectItems.Length)
                    {  //we iterated thourgh everything

                    }
                    if (subjectItemsIterator == subjectItems.Length-1)
                    {  //there is a single subjectitem left, we make this one its onwn subject ranking before continuing
                        RankingPlace rp = new RankingPlace();
                        rp.itemsThisRanking.Append(subjectItems[subjectItemsIterator]);
                        subranking.rankedHighToLow.Append(rp);
                        subRankings.Append(subranking);
                    }
                    state = State.Merging;
                    subRankingIndex = 0;
                    PrepareMergingStep();
                }
            }
            else if (state == State.Merging)
            {

            }
            else if (state == State.finished)
            {
            }
        }

        void PrepareMergingStep()
        {
            creatieRuimte = new Subranking();
            Source1Iterator = 0;
            Source2Iterator = 0;
            MergeSource1 = subRankings[subRankingIndex % subRankings.Length];
            MergeSource2 = subRankings[(subRankingIndex+1)% subRankings.Length];
        }

        public List<subjectItem> GetFinalRankedList()
        {
            return new List<subjectItem>();
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
