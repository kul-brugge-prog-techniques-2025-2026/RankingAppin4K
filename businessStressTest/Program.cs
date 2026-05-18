using business;
using Models;
using persistentie;

namespace businessStressTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //brute force random inputs
            PersistenceObject pers = new PersistenceObject();
            Random r = new Random();
            int smallestListYet = int.MaxValue;
            int normalSize = pers.GetSubjectItems(1).Count;
            for (int i = 0; i < 99999; i++)
            {
                Business b = new Business(1, pers);
                List<int> losers = new List<int>();//number one cannot be in here
                List<int> winners = new List<int>();//the lowest cannot be in here
                int tiecounter = 0;
                while (true)
                {
                    subjectItem[] sis = b.Give_options();
                    if (sis == null || sis.Length < 2)
                    {
                        break;
                    }
                    switch (r.Next(2))  //because you can end up above someone you lost to with ties, you can expect errors 1 and 2 if ties are allowed
                    {
                        case 0:
                            b.Give_result(new subjectItem[] { sis[0], sis[1] }, false);
                            losers.Remove(sis[1].Id);
                            losers.Add(sis[1].Id);
                            winners.Remove(sis[0].Id);
                            winners.Add(sis[0].Id);
                            break;
                        case 1:
                            b.Give_result(new subjectItem[] { sis[1], sis[0] }, false);
                            losers.Remove(sis[0].Id);
                            losers.Add(sis[0].Id);
                            winners.Remove(sis[1].Id);
                            winners.Add(sis[1].Id);
                            break;
                        case 2:
                            b.Give_result(new subjectItem[] { sis[0], sis[1] }, true);
                            tiecounter++;
                            break;
                    }
                }
                List<RankingItem> rank = b.GetFinalRankedList();
                int worstRank = rank.MaxBy(x => x.Rank).Rank;
                foreach (RankingItem item in rank) {
                    if (item.Rank == 0 && losers.Contains(item.subjectItemId))
                    {
                        Console.WriteLine("Error 1 in iteration: " + i);
                    }
                    if (item.Rank == worstRank && winners.Contains(item.subjectItemId))
                    {
                        Console.WriteLine("Error 2 in iteration: " + i);
                    }
                }
                if (normalSize - tiecounter > worstRank + 1)
                {
                    Console.WriteLine("Error 3 in iteration: " + i);
                }
                if (worstRank < smallestListYet)
                {
                    Console.WriteLine("New smallest list: " + worstRank + " ,iteration: " + i);
                    smallestListYet = worstRank;
                }
            }
        }
    }
}
