using Models;
using persistentie;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        int Mergingcounter;    //how many merges have happened
        DirectComparator CurrentComparison;

        int worstcaseComparisonsNeeded;
        List<Int32> WorstCaseMergeSteps;

        PersistenceObject opslag { get; set; }
        int subjectId { get; set; }
        Random r { get; set; }
        public Business(int subjectId, PersistenceObject persistence, List<int>? categoryids=null)
        {
            opslag = persistence;
            this.subjectId = subjectId;
            r = new Random();
            subjectItems = opslag.GetSubjectItems(subjectId);
            subjectItem[] scopy = subjectItems.ToArray();
            if (categoryids != null)
            {
                foreach (subjectItem s in scopy)
                {
                    bool keep = false;
                    foreach (int categoryid in s.Category)
                    {
                        if (categoryids.Contains(categoryid))
                        {
                            keep = true;
                        }
                    }
                    if (!keep)
                    {
                        subjectItems.Remove(s);
                    }
                }
            }
            if(subjectItems.Count <= 1)
            {
                throw new Exception() { Source = "Er moet meer dan 1 item zijn" };
            }
            this.WorstCaseMergeSteps = new List<int>();
            state = State.SRankCreating;
            subjectItemsIterator = 0;
            subRankings = new List<Subranking>();
            comparisonsNeeded(subjectItems.Count);
        }

        public Business()//test initialisation
        {
            subjectId = 0;
            r = new Random();
            subjectItems = new List<subjectItem>();
            for (int i = 0; i < 15; i++)
            {
                subjectItem item = new subjectItem { Id = (i / 2), Image = "", Text = new String[] { (i / 2).ToString() }, SubjectId = subjectId, Category = new int[] { 0 } };
                subjectItems.Add(item);
            }
            for (int i = 0; i < 15; i++)//sjuffel array
            {
                int rand = r.Next(14);
                subjectItem temp = subjectItems[rand];
                subjectItems[rand] = subjectItems[i];
                subjectItems[i] = temp;
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
                    Mergingcounter = 0;
                    PrepareMergingStep();
                }
            }
            else if (state == State.Merging)
            {
                CurrentComparison.propagateWinner(ranked[0], tied);
                
                if (CurrentComparison.done)
                {
                    creatieRuimte.rankedHighToLow.AddRange(CurrentComparison.RankedReturn().Take(CurrentComparison.RankedReturn().Count-1));
                    if(CurrentComparison.reapearing() == false)
                    {
                        Source1Iterator++;
                        Source2Iterator++;
                        creatieRuimte.rankedHighToLow.AddRange(CurrentComparison.RankedReturn().Skip(CurrentComparison.RankedReturn().Count - 1));
                    }
                    else
                    {
                        if (CurrentComparison.LosingStack() == 2)
                        {
                            Source1Iterator++;
                            MergeSource2.rankedHighToLow[Source2Iterator] = CurrentComparison.RankedReturn().TakeLast(1).ToArray()[0];//replace
                        }
                        else
                        {
                            Source2Iterator++;
                            MergeSource1.rankedHighToLow[Source1Iterator] = CurrentComparison.RankedReturn().TakeLast(1).ToArray()[0];
                        }
                    }
                    if (Source1Iterator == MergeSource1.rankedHighToLow.Count)//no need to ask anymore for this merge, we know the result
                    {
                        creatieRuimte.rankedHighToLow.AddRange(MergeSource2.rankedHighToLow.Skip(Source2Iterator));
                        FinishMergingStepAndNew();
                    }
                    else

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
            else if (state == State.finished)
            {
            }
        }

        void FinishMergingStepAndNew()
        {
            subRankings.Remove(MergeSource1);
            subRankings.Remove(MergeSource2);
            subRankings.Add(creatieRuimte);
            Mergingcounter++;
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
            MergeSource1 = subRankings[0];
            MergeSource2 = subRankings[1];
            CurrentComparison = new DirectComparator(MergeSource1.rankedHighToLow[Source1Iterator], MergeSource2.rankedHighToLow[Source2Iterator]);
        }
        public double GetCompletionPercentage()
        {
            if (state == State.SRankCreating)
            {
                return (((double)subjectItemsIterator / 2) / worstcaseComparisonsNeeded)*100;
            }
            else if (state == State.Merging)
            {
                return (((double)WorstCaseMergeSteps[Mergingcounter] + Source1Iterator+Source2Iterator)/ worstcaseComparisonsNeeded) * 100;
            }
            else if (state == State.finished)
            {
                return 100;
            }
            return 0;
        }

        public List<RankingItem> GetFinalRankedList()
        {
            if(state != State.finished)
            {
                return new List<RankingItem>();
            }
            //everything should be in subrankings[0]
            var list = new List<RankingItem>();
            int idCounter = 1;
            for ( int i = 0; i < subRankings[0].rankedHighToLow.Count; i++ ) {
                foreach (subjectItem si in subRankings[0].rankedHighToLow[i].itemsThisRanking)
                {
                    RankingItem RA = new RankingItem();
                    RA.Id = idCounter++;
                    RA.subjectitem = si;
                    RA.Rank = i;
                    RA.subjectItemId = si.Id;
                    list.Add(RA);
                }
            }
            return list;
        }

        public void SaveCurrent(string userName)
        {
            RankingResult result = new RankingResult() {Name = userName};
            result.RankedItems = GetFinalRankedList();
            result.SubjectId = subjectId;
            opslag.SaveRanking(userName, subjectId, GetFinalRankedList());

        }

        public List<Subject> GiveAllSubjects()
        {
            //return null;
            return opslag.GiveAllSubjects();
        }

        public List<RankingResult> GetSavedRankings()
        {
            var results = opslag?.RetrieveRankings(subjectId) ?? new List<RankingResult>();
            var allItemsForLookup = opslag.GetSubjectItems(subjectId);
            var lookup = allItemsForLookup.ToDictionary(si => si.Id);

            foreach (var res in results)
            {
                foreach (var ri in res.RankedItems)
                {
                    if (lookup.TryGetValue(ri.subjectItemId, out var si))
                    {
                        ri.subjectitem = si;
                    }
                }
            }

            return results;
        }

        public List<RankingItem> GetRankingItemsForResult(int rankingResultId)
        {
            return opslag?.GetRankingItemsForResult(rankingResultId) ?? new List<RankingItem>();
        }

        public double Compare(RankingResult r, RankingResult r2)
        {
            List<RankingItem> ri = opslag.GetRankingItemsForResult(r.Id);
            List<RankingItem> ri2 = opslag.GetRankingItemsForResult(r2.Id);
            var comparingpositions2 = (from item in ri2 orderby item.Rank select item.Rank).ToArray();
            var comparingpositions = (from item in ri orderby item.Rank select item.Rank).ToArray();
            var comparingIds2 = (from item in ri2 orderby item.Rank select item.subjectItemId).ToArray();
            var comparingIds = (from item in ri orderby item.Rank select item.subjectItemId).ToArray();
            double similairty = compareResults(comparingpositions, comparingpositions2, comparingIds, comparingIds2, ri.Count, ri2.Count,comparingpositions.Last() + 1, comparingpositions2.Last() + 1);
            return  similairty;
        }
        public double Compare(RankingResult r2)
        {
            List<RankingItem> ri = GetFinalRankedList();
            List<RankingItem> ri2 = opslag.GetRankingItemsForResult(r2.Id);
            var comparingpositions2 = (from item in ri2 orderby item.Rank select item.Rank).ToArray();
            var comparingpositions = (from item in ri orderby item.Rank select item.Rank).ToArray();
            var comparingIds2 = (from item in ri2 orderby item.Rank select item.subjectItemId).ToArray();
            var comparingIds = (from item in ri orderby item.Rank select item.subjectItemId).ToArray();
            double similairty = compareResults(comparingpositions,comparingpositions2, comparingIds, comparingIds2, ri.Count, ri2.Count, comparingpositions.Last()+1, comparingpositions2.Last()+1);//+1 because it is zerobased data, but the math expects non zerobased rankings
            return similairty;
        }
        public void comparisonsNeeded(int items)//mimics the algorithm to see at which checkpoints (end of merge) we have a certain completion.
        {
            int total = 0;
            int i = 0;
            List<Int32> GroupSizes = new List<int>();
            total += items / 2; //floor
            WorstCaseMergeSteps.Add(items / 2);
            while (true)
            {
                GroupSizes.Add(2);
                i += 2;
                if(i == items) {  break; }
                if(i == items - 1) { GroupSizes.Add(1); break; }
            }
            while (GroupSizes.Count != 1)
            {
                total += GroupSizes[0] + GroupSizes[1] - 1;
                WorstCaseMergeSteps.Add(total);
                int newsize = GroupSizes[0] + GroupSizes[1];
                GroupSizes.RemoveRange(0, 2);
                GroupSizes.Add(newsize);
            }
            worstcaseComparisonsNeeded = total;
        }
        [DllImport("RankingComparing.dll")]
        public static extern double compareResults(int[] positions1, int[] positions2, int[] ids1, int[] ids2, int length1, int length2, int maxranking1, int maxranking2);
    }
}
