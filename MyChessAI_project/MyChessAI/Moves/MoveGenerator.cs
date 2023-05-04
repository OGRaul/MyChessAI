using System;

public struct Move
{
    int startPosition;
    char piece;
    int pieceColor;
    int endPosition;
}

namespace Moves
{
    public class MoveGenerator
    {
        List<Move> moves = new List<Move>();

        //generates a list of all legal moves
        public List<Move> generateLegalMoves()
        {
            //TODO: list all legal moves

            return moves;
        }
    }
}