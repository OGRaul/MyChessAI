using System;
using Moves;
using Board;

namespace Engine
{
    public class RandomAI
    {
        public string playMove()
        {
            //gets all legal moves
            List<string> possibleMoves = MoveGenerator.findAllLegalMoves();
            
            //plays random move
            Random r = new Random();
            int randomMoveIndex = r.Next(possibleMoves.Count-1);

            string move = possibleMoves[randomMoveIndex];
            
            return BoardManager.makeMove(move);
        }
    }
}