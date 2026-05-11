using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace persistentie//guys i think my ai is broken
{
    public class PersistenceObject
    {
        private string subjectFilePath = "json_files/Subjects.json";

        private string rankingsFolder = AppDomain.CurrentDomain.BaseDirectory;

        private class SubjectsWrappper { public List<Subject> Subjects { get; set; } }
        private class ItemsWrapper { public List<subjectItem> Items { get; set; } }

        //Alle thema's ophalen
        public List<Subject> Give_all_subjects()
        {
            if (!File.Exists(subjectFilePath)) return new List<Subject>();  
            //Lees volledig bestand uit
            string json = File.ReadAllText(subjectFilePath);
            //converteer de tekst naar list van subjects
            var wrapper = JsonSerializer.Deserialize<SubjectsWrappper>(json);
            return wrapper?.Subjects ?? new List<Subject>();
        }

        //Alle items voor specifieke categorie ophalen
        public List<subjectItem> GetSubjectItems(int subjectId)
        {
            var subject = Give_all_subjects().FirstOrDefault(s => s.Id == subjectId);

            if (subject == null) return new List<subjectItem>();

            string specificFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{subject.Name}.json");

            if (!File.Exists(specificFileName)) return new List<subjectItem>();

            string json = File.ReadAllText(specificFileName);
            var wrapper = JsonSerializer.Deserialize<ItemsWrapper>(json);

            return wrapper?.Items ?? new List<subjectItem>();
        }

        //Haalt alle opgeslagen rankings op voor een categorie
        public List<RankingResult> retrieve_rankings(int subjectId)
        {
            List<RankingResult> results = new List<RankingResult>();

            string[] files = Directory.GetFiles(rankingsFolder, "ranking_*.json");

            foreach (string file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    RankingResult res = JsonSerializer.Deserialize<RankingResult>(json);

                    if (res != null && res.SubjectId == subjectId)
                    {
                        results.Add(res);
                    }
                }
                catch (Exception) { }
            }
            return results;
        }

        //Haalt de specifieke rank op van één sessie
        public List<RankingItem> GetRankingItemsForResult(int rankingResultId)
        {
            string fileName = Path.Combine(rankingsFolder, $"ranking_{rankingResultId}.json");

            if (!File.Exists(fileName)) return new List<RankingItem>();

            string json = File.ReadAllText(fileName);
            RankingResult res = JsonSerializer.Deserialize<RankingResult>(json);

            return res?.RankedItems ?? new List<RankingItem>();
        }

        public void saveRanking(string name, int subjectId, List<RankingItem> rankedList)   //Dit moet rankingitem zijn omdat er in het eindresultaat nogsteeds gelijke posities kunnen zitten.
        {
            string[] existingFiles = Directory.GetFiles(rankingsFolder, "ranking_*.json");
            int newId = 1;
            if (existingFiles.Length > 0)
            {
                newId = existingFiles.Length + 1;
            }

            RankingResult newResult = new RankingResult
            {
                Id = newId,
                SubjectId = subjectId,
                Name = name,
                RankedItems = new List<RankingItem>()
            };

            for (int i = 0; i < rankedList.Count; i++ )
            {
                newResult.RankedItems.Add(new RankingItem
                {
                    Id = i + 1,
                    subjectItemId = rankedList[i].Id,
                    RankingResultId = newId,
                    Rank = i + 1
                });
            }

            string jsonString = JsonSerializer.Serialize(newResult, new JsonSerializerOptions { WriteIndented = true });

            string fileName = Path.Combine(rankingsFolder, $"ranking_{newId}.json");
            File.WriteAllText(fileName, jsonString);
        }
    }
}
