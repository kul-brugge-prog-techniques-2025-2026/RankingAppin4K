using persistentie;
using Models;

namespace persistentieTests
{
    [TestClass]
    public class PersistenceTests
    {
        private PersistenceObject _persistence;
        private string _testDir;

        [TestInitialize]
        public void Setup()
        {
            //Maakt tijdeijke map aan
            _testDir = Path.Combine(Path.GetTempPath(), "RankingAppTests_" + Guid.NewGuid().ToString());
            string jsonDir = Path.Combine(_testDir, "json_files");
            Directory.CreateDirectory(jsonDir);

            //dummy Subjects.json
            string subjectsJson = "{ \"Subjects\": [ { \"Id\": 1, \"Name\": \"TestSubject\", \"Photo\": \"test.jpg\" } ] }";
            File.WriteAllText(Path.Combine(jsonDir, "Subjects.json"), subjectsJson);

            //Dummy TestSubjects.json
            string itemsJson = "{ \"Items\": [ { \"Id\": 1, \"SubjectId\": 1, \"Text\": [\"Item1\"], \"Image\": \"img1.jpg\" } ] }";
            File.WriteAllText(Path.Combine(jsonDir, "TestSubject.json"), itemsJson);

            _persistence = new PersistenceObject();
        }

        [TestMethod]
        public void SaveAndRetrieveRanking()
        {
            string testName = "UnitTester_" + Guid.NewGuid().ToString();
            int subjectId = 1;
            var items = new List<RankingItem>
            {
                new RankingItem { Id = 1, Rank = 1, subjectItemId = 10 }
            };

            _persistence.SaveRanking(testName, subjectId, items);

            var allRankings = _persistence.RetrieveRankings(subjectId);
            var savedRecord = allRankings.FirstOrDefault(r => r.Name == testName);

            Assert.IsNotNull(savedRecord);
            Assert.AreEqual(testName, savedRecord.Name);
            Assert.AreEqual(subjectId, savedRecord.SubjectId);
        }

        [TestMethod]
        public void GetSubjectItems()
        {
            var subjects = _persistence.GiveAllSubjects();
            if (subjects.Count > 0)
            {
                var items = _persistence.GetSubjectItems(subjects[0].Id);

                if (items.Count > 0)
                {
                    Assert.IsTrue(Path.IsPathRooted(items[0].Image));
                }
            }
        }

        [TestMethod]
        public void GetRankingItemsForResult()
        {
            var result = _persistence.GetRankingItemsForResult(999999);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count, "Returned empty list");
        }
    }
}
