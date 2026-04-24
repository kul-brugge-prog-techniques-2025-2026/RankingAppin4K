using Models;
using persistentie;
using System;
using System.Collections.Generic;
using System.Text;

namespace business
{
    public class Business
    {
        PersistenceObject opslag { get; set; }
        int subjectId { get; set; }
        public Business(int subjectId, PersistenceObject persistence)
        {
            opslag = persistence;
            subjectId = subjectId;
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
            return null;
        }
    }
}
