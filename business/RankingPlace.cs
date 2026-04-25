using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace business
{
    internal class RankingPlace
    {
        public RankingPlace()
        {
            itemsThisRanking = [];
        }
        public subjectItem[] itemsThisRanking; //because we need to handle ties, they will be able to get untied when merging, because both get asked, if one wins and the other loses than we get an untie.
    }
}
