using System;

namespace Moves
{
    public static class PositionOffsets
    {
        public const int upOne = 8;
        public const int downOne = -8;
        public const int rightOne = 1;
        public const int leftOne = -1;
        public const int upLeft = 7;
        public const int downRight = -7;
        public const int upRight = 9;
        public const int downLeft = -9;

        public static int[] vertHorz = {upOne, downOne, rightOne, leftOne};
        public static int[] diagonals = {upLeft, downRight, upRight, downLeft};

        public static int[] allDirections = {upOne, downOne, rightOne, leftOne, upLeft, downRight, upRight, downLeft};

        public static int[] horseDirections = 
        {
            (upOne*2) + leftOne, //0 upleft
            (upOne*2) + rightOne, //1 upright

            (downOne*2) + leftOne, //2 downleft
            (downOne*2) + rightOne, //3 downright

            (rightOne*2) + upOne, //4 righup
            (rightOne*2) + downOne, //5 rightdown

            (leftOne*2) + upOne, //6 leftup
            (leftOne*2) + downOne //7 leftdown
        };

        public static int getNumSquaresToEdge(int position, int direction)
        {
            int numSquares = 0;

            for(int i = 1; i <= 8; i++)
            {
                if(position + direction < 64 && position + direction >= 0)
                {
                    numSquares++;
                }
            }

            return numSquares;
        }
    }
}