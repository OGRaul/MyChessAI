 using System;

namespace Resources
{
    public static class Ranks
    {
 
        //ranks
        public static int[] firstRank = {56, 57, 58, 59, 60, 61, 62, 63};
        public static int[] secondRank = {48, 49, 50, 51, 52, 53, 54, 55};
        public static int[] thirdRank = {40, 41, 42, 43, 44, 45, 46, 47};
        public static int[] fourthRank = {32, 33, 34, 35, 36, 37, 38, 39};
        public static int[] fifthRank = {24, 25, 26, 27, 28, 29, 30, 31};
        public static int[] sixthRank = {16, 17, 18, 19, 20, 21, 22, 23};
        public static int[] seventhRank = {8, 9, 10, 11, 12, 13, 14, 15};
        public static int[] eigthRank = {0, 1, 2, 3, 4, 5, 6, 7};

        //A 2d array to easily get all ranks
        public static int[][] allRanks = 
        {
            firstRank, secondRank, thirdRank, fourthRank,
            fifthRank, sixthRank, seventhRank, eigthRank
        };

    }
}