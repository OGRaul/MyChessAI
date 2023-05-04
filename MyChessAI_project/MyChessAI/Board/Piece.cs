using System;

namespace Board
{
    public class Piece
    {
        public const char PIECENONE = '-';
        public const char PAWN = 'p';
        public const char BISHOP = 'b';
        public const char KNIGHT = 'n';
        public const char ROOK = 'r';
        public const char QUEEN = 'q';
        public const char KING = 'k';

        public const int COLORNONE = 0;
        public const int WHITE = 8;
        public const int BLACK = 16;
    }
}