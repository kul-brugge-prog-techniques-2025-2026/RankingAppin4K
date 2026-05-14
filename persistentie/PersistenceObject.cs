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
        private string _subjectFilePath = "json_files/Subjects.json";
        private string _rootDataPath;
        private string _rankingsFolder;

        private const int MaxSearchDepth = 6;

        private class SubjectsWrapper { public List<Subject> Subjects { get; set; } }
        private class ItemsWrapper { public List<subjectItem> Items { get; set; } }

        public PersistenceObject()
        {
            InitializeDataPaths();
            EnsureFoldersExist();
        }

        private void InitializeDataPaths()
        {
            string currentDir = AppContext.BaseDirectory;
            string? foundPath = null;

            for (int i = 0; i < MaxSearchDepth; i++)
            {
                string testPath = Path.Combine(currentDir, "json_files");
                if (Directory.Exists(testPath))
                {
                    foundPath = testPath;
                    break;
                }
                var parent = Directory.GetParent(currentDir);
                if (parent == null) break;
                currentDir = parent.FullName;
            }

            if (foundPath == null)
            {
                throw new FileNotFoundException("De map 'json_files' kon niet worden gevonden. " +
                    "Zorg dat deze in de projectmap staat of gekopieerd wordt naar de output directory.");
            }

            _rootDataPath = foundPath;

            _rankingsFolder = Path.Combine(Directory.GetParent(_rootDataPath).FullName, "savedRankings");
        }

        private void EnsureFoldersExist()
        {
            if (!Directory.Exists(_rankingsFolder))
            {
                Directory.CreateDirectory(_rankingsFolder);
            }
        }

        //Alle thema's ophalen
        public List<Subject> GiveAllSubjects()
        {
            if (!File.Exists(_subjectFilePath)) return new List<Subject>();  

            string json = File.ReadAllText(_subjectFilePath);
            //converteer de tekst naar list van subjects
            var wrapper = JsonSerializer.Deserialize<SubjectsWrapper>(json);
            return wrapper?.Subjects ?? new List<Subject>();
        }

        //Alle items voor specifieke categorie ophalen
        public List<subjectItem> GetSubjectItems(int subjectId)
        {
            var allSubjects = GiveAllSubjects();

            var currentSubject = allSubjects.FirstOrDefault(s => s.Id == subjectId);

            if (currentSubject == null) return new List<subjectItem>();

            string subjectsDir = Path.GetDirectoryName(_subjectFilePath) ?? AppDomain.CurrentDomain.BaseDirectory;
            string specificFileName = Path.Combine(subjectsDir, $"{currentSubject.Name}.json");

            if (!File.Exists(specificFileName)) return new List<subjectItem>();

            string json = File.ReadAllText(specificFileName);
            var wrapper = JsonSerializer.Deserialize<ItemsWrapper>(json);
            var items = wrapper?.Items ?? new List<subjectItem>();

            string projectRoot = Directory.GetParent(_rootDataPath).FullName;
            foreach (var item in items)
            {
                if (!Path.IsPathRooted(item.Image))
                {
                    item.Image = Path.GetFullPath(Path.Combine(projectRoot, item.Image));
                }
            }
            return items;
        }

        //Haalt alle opgeslagen rankings op voor een categorie
        public List<RankingResult> RetrieveRankings(int subjectId)
        {
            List<RankingResult> results = new List<RankingResult>();

            string[] files = Directory.GetFiles(_rankingsFolder, "ranking_*.json");

            foreach (string file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    RankingResult rankingResult = JsonSerializer.Deserialize<RankingResult>(json);

                    if (rankingResult != null && rankingResult.SubjectId == subjectId)
                    {
                        results.Add(rankingResult);
                    }
                }
                catch (Exception ex)
                { 
                    Console.WriteLine(ex.Message);
                }
            }
            return results;
        }

        //Haalt de specifieke rank op van één sessie
        public List<RankingItem> GetRankingItemsForResult(int rankingResultId)
        {
            string fileName = Path.Combine(_rankingsFolder, $"ranking_{rankingResultId}.json");

            if (!File.Exists(fileName)) return new List<RankingItem>();

            string jsonContent = File.ReadAllText(fileName);
            RankingResult res = JsonSerializer.Deserialize<RankingResult>(jsonContent);

            return res?.RankedItems ?? new List<RankingItem>();
        }

        public void SaveRanking(string name, int subjectId, List<RankingItem> rankedList)
        {
            int newId = Directory.GetFiles(_rankingsFolder, "ranking_*.json").Length + 1;

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

            string fileName = Path.Combine(_rankingsFolder, $"ranking_{newId}.json");
            File.WriteAllText(fileName, jsonString);
        }
    }
}
