using System.Security.Cryptography.X509Certificates;
using Models;

namespace persistentie
{
    public class Program
    {
        public Program()
        {
            Console.WriteLine("hi");
            test();
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("hi");
            test();
        }
        public static void test()
        {
            try
            {
                Console.WriteLine("Huidige map: " + Directory.GetCurrentDirectory());

                PersistenceObject po = new PersistenceObject();

                Console.WriteLine("\n--- Laden van subjects ---");

                List<Subject> subjects = po.Give_all_subjects();

                Console.WriteLine($"Aantal subjects gevonden: {subjects.Count}");

                foreach (var subject in subjects)
                {
                    Console.WriteLine($"- {subject.Name}");
                }

                if (subjects.Count == 0)
                    return;

                int subjectId = subjects[0].Id;

                Console.WriteLine($"\n--- Laden van items voor {subjects[0].Name} ---");

                List<subjectItem> items = po.Get_SubjectItems(subjectId);

                Console.WriteLine($"Aantal items gevonden: {items.Count}");

                if (items.Count < 5)
                {
                    Console.WriteLine("Niet genoeg items om top 5 te maken.");
                    return;
                }

                Console.WriteLine("\nTop 5 items:");

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"{i + 1}. {items[i].Text[0]}");
                }

                Console.WriteLine("\n--- Aanmaken van ranking ---");

                List<RankingItem> top5List = new List<RankingItem>();

                for (int i = 0; i < 5; i++)
                {
                    top5List.Add(new RankingItem
                    {
                        Id = i + 1,
                        Rank = i + 1,
                        subjectItemId = items[i].Id
                    });
                }

                po.saveRanking("TestRanking", subjectId, top5List);

                Console.WriteLine("Ranking opgeslagen!");

                Console.WriteLine("\n--- Opgeslagen rankings ophalen ---");

                var savedRankings = po.retrieve_rankings(subjectId);

                Console.WriteLine($"Aantal rankings gevonden: {savedRankings.Count}");

                foreach (var ranking in savedRankings)
                {
                    Console.WriteLine($"\nRanking: {ranking.Name}");
                    Console.WriteLine($"ID: {ranking.Id}");

                    var rankingItems = po.GetRankingItemsForResult(ranking.Id);

                    Console.WriteLine("Items:");

                    foreach (var item in rankingItems.OrderBy(i => i.Rank))
                    {
                        Console.WriteLine(
                            $"Rank {item.Rank} -> SubjectItemId {item.subjectItemId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FOUT: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Datail: {ex.InnerException.Message}");
                }
            }
        }
    }
}
