using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace persistentie
{
    public class PersistenceObject
    {
        private string subjectFilePath = "json_files/Subjects.json";

        private string rankingsFolder = "savedRankings";

        private class SubjectsWrapper { public List<Subject> Subjects { get; set; } }
        private class ItemsWrapper { public List<subjectItem> Items { get; set; } }

        public PersistenceObject()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            DirectoryInfo projectDir = Directory.GetParent(baseDir).Parent.Parent.Parent;

            subjectFilePath = Path.Combine(projectDir.FullName, "json_files", "subjects.json");
            rankingsFolder = Path.Combine(projectDir.FullName, "savedRankings");
        }

        //Alle thema's ophalen
        public List<Subject> Give_all_subjects()
        {
            if (!File.Exists(subjectFilePath)) return new List<Subject>();  
            //Lees volledig bestand uit
            string json = File.ReadAllText(subjectFilePath);
            //converteer de tekst naar list van subjects
            var wrapper = JsonSerializer.Deserialize<SubjectsWrapper>(json);
            return wrapper?.Subjects ?? new List<Subject>();
        }

        //Alle items voor specifieke categorie ophalen
        public List<subjectItem> Get_subjectItems(int subjectId)
        {
            var allSubjects = Give_all_subjects();

            var currentSubject = allSubjects.FirstOrDefault(s => s.Id == subjectId);

            if (currentSubject == null) return new List<subjectItem>();

            string specificFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json_files",  $"{currentSubject.Name}.json");

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
                catch { }
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

        public void saveRanking(string name, int subjectId, List<RankingItem> rankedList)
        {
            int newId = Directory.GetFiles(rankingsFolder, "ranking_*.json").Length + 1;

            foreach (var item in rankedList)
            {
                item.RankingResultId = newId;
            }

            RankingResult newResult = new RankingResult
            {
                Id = newId,
                SubjectId = subjectId,
                Name = name,
                RankedItems = rankedList
            };

            string jsonString = JsonSerializer.Serialize(newResult, new JsonSerializerOptions { WriteIndented = true });

            string fileName = Path.Combine(rankingsFolder, $"ranking_{newId}.json");
            File.WriteAllText(fileName, jsonString);
        }
    }
}
