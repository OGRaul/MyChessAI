using System;
using Board;

namespace Moves
{
    public class MoveInterpreter
    {
        public bool isValidMove(string move)
        {
            if(move.Length > 5 || move.Length < 4)
            {
                return false;
            }
            else if(move[0] < 'a' || move[0] > 'h' ||
                    move[1] < '1' || move[1] > '8' ||
                    move[2] < 'a' || move[2] > 'h' ||
                    move[3] < '1' || move[3] > '8')
            {
                return false;
            }
            else if(move.Length == 5 &&
                    move[4] != '\x0000' &&  move[4] != ' ' &&
                    move[4] != 'q' &&  move[4] != 'Q' &&
                    move[4] != 'n' &&  move[4] != 'N' &&
                    move[4] != 'b' &&  move[4] != 'B' &&
                    move[4] != 'r' &&  move[4] != 'R')
            {
                return false;
            }

            return true;
        }
    }
}