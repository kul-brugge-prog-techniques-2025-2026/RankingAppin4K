using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace persistentie
{
    public class PersistenceObject
    {
        private string resultFilePath = "";
        private string itemsFilePath = "";
        private string subjectFilePath = "";
        private string subjectItemsFilePath = "";

        //Alle thema's ophalen
        public List<Subject> Give_all_subjects()
        {
            if (!File.Exists(subjectFilePath)) return new List<Subject>();
            //Lees volledig bestand uit
            string json = File.ReadAllText(subjectFilePath);
            //converteer de tekst naar list van subjects
            return JsonSerializer.Deserialize<List<Subject>>(json);
        }

        //Alle items voor specifieke categorie ophalen
        public List<subjectItem> GetSubjectItems(int subjectId)
        {
            if (!File.Exists(subjectItemsFilePath)) return new List<subjectItem>();

            string json = File.ReadAllText(subjectItemsFilePath);
            var allItems = JsonSerializer.Deserialize<List<subjectItem>>(json);
            return allItems.Where(i => i.SubjectId == subjectId).ToList();
        }

        //Haalt alle opgeslagen rankings op voor een categorie
        public List<RankingResult> retrieve_rankings(int SubjectId)
        {
            if (!File.Exists(resultFilePath)) return new List<RankingResult>();

            string resultsJson = File.ReadAllText(resultFilePath);
            List<RankingResult> allResults = JsonSerializer.Deserialize<List<RankingResult>>(resultsJson);

            //Filtert alle rankings zodat enkel de resultaten met de categorie overschiet
            List<RankingResult> filteredResults = allResults
                .Where(r => r.SubjectId == SubjectId)
                .ToList();

            return filteredResults;
        }

        //Haalt de specifieke rank op van één sessie
        public List<RankingItem> GetRankingItemsForResult(int rankingResultId)
        {
            if (!File.Exists(itemsFilePath)) return new List<RankingItem>();

            string ItemsJson = File.ReadAllText(itemsFilePath);
            List<RankingItem> allItems = JsonSerializer.Deserialize<List<RankingItem>>(ItemsJson);

            return allItems.Where(i => i.RankingResultId == rankingResultId).ToList();
        }

        public void saveRanking(string name, int subjectId, subjectItem[] rankedList)
        {
            List<RankingResult> allResults = new List<RankingResult>();
            if (File.Exists(resultFilePath) )
            {
                string resultsJson = File.ReadAllText(resultFilePath);
                allResults = JsonSerializer.Deserialize<List<RankingResult>>(resultsJson) ?? new List<RankingResult>();
            }

            //bep nieuwe id auto increment
            int newResultId = allResults.Count > 0 ? allResults.Max(r => r.Id) + 1 : 1;

            RankingResult newResult = new RankingResult
            {
                Id = newResultId,
                SubjectId = subjectId,
                Name = name
            };

            allResults.Add(newResult);
            string updatedResultJson = JsonSerializer.Serialize(allResults, new JsonSerializerOptions { WriteIndented = true }); 
            File.WriteAllText(resultFilePath, updatedResultJson);

            List<RankingItem> allRankingItems = new List<RankingItem>();
            if (File.Exists(itemsFilePath))
            {
                string itemsJson = File.ReadAllText(itemsFilePath);
                allRankingItems = JsonSerializer.Deserialize<List<RankingItem>>(itemsJson) ?? new List<RankingItem>();
            }

            int nextItemId = allRankingItems.Count > 0 ? allRankingItems.Max(i => i.Id) + 1 : 1;

            for (int i = 0; i < rankedList.Length; i++)
            {
                RankingItem rankingEntry = new RankingItem
                {
                    Id = nextItemId,
                    RankingResultId = newResultId,
                    subjectItemId = rankedList[i].Id,
                    Rank = i + 1
                };

                allRankingItems.Add(rankingEntry);
                nextItemId++;
            }

            string updateItemJson = JsonSerializer.Serialize(allRankingItems, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(itemsFilePath, updateItemJson);
        }
    }
}
