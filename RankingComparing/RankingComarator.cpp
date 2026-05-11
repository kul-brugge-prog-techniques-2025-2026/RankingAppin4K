#include "pch.h"
#include "RankingComarator.h"
#include <iostream>

// there is a function called compare(int[], int[], int[], int[]) which contains pointers to arrays, or arrays (idk depends on how the conversion works), these are array pairs, one is for the position of the ranking, and another is for the id of the item

double compareResults(int* positions1, int* positions2, int* ids1, int* ids2, int length, int maxranking1, int maxranking2)   //in case of ties is the max ranking not the same as the length
{
    double pointperid = 100.f / (double)length;
    double totalsimilarity = 0;
    for (int i = 0; i < length; i++)
    {
        //in the array ranking and id are matched
        //average distance with 0 points starting from 50% distance in the array
        for (int i = 0; i < length; i++) {
            int id1 = ids1[i];
            int indexj;
            for (int j = 0; j < length; j++) {
                if (ids2[j] == id1) {
                    indexj = j;
                }
            }
            double half = 0.5;//we schale for different max positions
            double distance = abs(positions1[i]/maxranking1 - positions2[indexj]/maxranking2);
            if (distance < half) {
                totalsimilarity += pointperid * (1 - distance / half);
            }
        }
    }
    return totalsimilarity;
}