#include "pch.h"
#include "RankingComarator.h"
#include <iostream>
#include <algorithm>

//difference between item positions (with max 1/2 away) / number of items in the arrays (biggest of the 2)

double compareResults(int* positions1, int* positions2, int* ids1, int* ids2, int length1, int length2, int maxranking1, int maxranking2)   //in case of ties is the max ranking not the same as the length
{
    int maxlenght = max(length1, length2);
    double pointperid = 100.f / (double)maxlenght;
    double totalsimilarity = 0;
    //in the array ranking and id are matched
    //average distance with 0 points starting from 50% distance in the array
    for (int i = 0; i < length1; i++) {
        int id1 = ids1[i];
        int indexj = -1;
        for (int j = 0; j < length2; j++) {
            if (ids2[j] == id1) {
                indexj = j;
            }
        }
        if (indexj == -1) {
            continue;
        }
        double half = 0.5;//we schale for different max positions
        double distance = abs((double)positions1[i]/maxranking1 - (double)positions2[indexj]/maxranking2);
        if (distance < half) {
            totalsimilarity += pointperid * (1 - distance / half);
        }
    }
    return totalsimilarity;
}