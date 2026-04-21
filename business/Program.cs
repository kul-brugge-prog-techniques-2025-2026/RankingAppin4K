using Models;
using persistentie;

namespace business
{
    public class BusinessLayer
    {
        public BusinessLayer() 
        {

        }

        public BusinessLayer(int subjectId, PersistenceObject persistence)
        {

        }

        public subjectItem[] Give_options()
        {

            return new subjectItem[2];
        }

        public void Give_result(subjectItem[] ranked)
        {

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

        }
    }
}
