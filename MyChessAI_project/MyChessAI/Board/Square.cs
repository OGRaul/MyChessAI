using System;

namespace Board
{
    public class Square
    {
        public int position = -1;
        public bool hasMoved = false;
        public char piece {get; set;} = Piece.PIECENONE;

        private int _color = Piece.COLORNONE;
        public int color 
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                if(_color == Piece.WHITE)
                {
                    piece = Char.ToUpper(piece);
                }
                else if(_color == Piece.BLACK)
                {
                    piece = Char.ToLower(piece);
                }
            }
        }
        public bool isAtackedByWhite;
        public bool isAtackedByBlack;
    }
}