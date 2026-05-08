#include "pch.h"
#include "RankingComarator.h"
#include <iostream>

// there is a function called compare(int[], int[], int[], int[]) which contains pointers to arrays, or arrays (idk depends on how the conversion works), these are array pairs, one is for the position of the ranking, and another is for the id of the item



extern "C" __declspec(dllexport)
double compareResults(int* positions1, int* positions2, int* ids1, int* ids2, int length)
{
    for (int i = 0; i < length; i++)
    {
        //std::cout << values[i] << std::endl;
    }
}