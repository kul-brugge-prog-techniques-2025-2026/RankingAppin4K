# RankingAppin4K

## Klassen
Persistentie met volgende klassen:
- Subject
- subjectItem
- RankingResult
- RankingItem
Functie aanroepen:
- Give_all_Subjects()
- Get_subjectItems(int SubjectId)
- SaveRanking(String name, int subjectId, subjectItem[] rankedList)
- retrieve_rankings(int SubjectId)


Business laag:
- Istantieer baar in de ui
- Give_all_Subjects()
- Constructor(SubjectId, PersistanceObject o)
- subjectItem[2] Give_options()
- void Give_result(subjectItem[2] ranked)
- ComparedRankignResult[] Compare()
- SaveCurrent()

ComparedRankingResult:
    -erft van RankingResult
    -SimilarityRate: Double