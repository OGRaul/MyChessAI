using System;
using Board;
using Util;

public struct Move
{
    int startPosition;
    char piece;
    int pieceColor;
    int endPosition;
}

namespace Moves
{
    public static class MoveGenerator
    {
        public static List<string> legalMoves = new List<string>();
        private static string promotionPieceTypes = " qrnb";

        //TODO: test this
        public static List<string> findAllLegalMoves()
        {
            List<string> legalMoves = new List<string>();
            List<string> pseudoLegalMoves = generatePseudoLegalMoves();

            //looks to see which pseudolegalmoves are actually legal and saves them
            for(int i = 0; i < pseudoLegalMoves.Count; i++)
            {
                //saves the current position for later 
                string fenBefore = FenParser.createFenFromPosition();

                //makes the move being tested on the board
                BoardManager.makeMove(pseudoLegalMoves[i]);

                //generates responses to the move being tested
                List<string> oponentResponses = generatePseudoLegalMoves();
                
                //if the opoonent couldn't take your king after your move it was legal
                if(!couldOponentTakeYourKing(oponentResponses))
                {
                    legalMoves.Add(pseudoLegalMoves[i]);
                }

                //undoes the move
                FenParser.loadPositionFromFen(fenBefore);
            }

            return legalMoves;
        }

        //TODO: test this
        //generates a list of all legal moves
        private static List<string> generatePseudoLegalMoves()
        {
            List<string> pseudoLegalMoves = new List<string>();

            //runs through the board
            for(int i = 0; i < BoardManager.pieces.Length; i++)
            {
                //if a square contains a piece of the same color as the turn it finds all its legal moves
                if(BoardManager.pieces[i].piece != Piece.PIECENONE && BoardManager.pieces[i].color == BoardManager.currentTurnColor)
                {
                    int startPos = BoardManager.pieces[i].position;

                    for(int endPos = 0; endPos < BoardManager.pieces.Length; endPos++)
                    {
                        int endFile = getEndFileNumber(endPos);

                        for(int j = 0; j < promotionPieceTypes.Length; j++)
                        {
                            char? promotion;

                            if(promotionPieceTypes[j] == ' ')
                            {
                                promotion = null;
                            }
                            else
                            {
                                promotion = promotionPieceTypes[j];
                            }

                            if(BoardManager.isLegalMove(startPos, endPos, endFile, promotion))
                            {
                                string move = indexPositionToCoordinate(startPos) + indexPositionToCoordinate(endPos) + promotion;
                                pseudoLegalMoves.Add(move);
                            }
                        }
                    }
                }
            }

            return pseudoLegalMoves;
        }

        private static bool couldOponentTakeYourKing(List<string> oponentResponses)
        {
            for(int i = 0; i < oponentResponses.Count; i++)
            {
                string endSquare = "";
                endSquare += oponentResponses[i][2];
                endSquare += oponentResponses[i][3];

                int endSquareIndex = BoardManager.coordinateToIndexPosition(BoardManager.columnLetterToNumber(oponentResponses[i][2]), (int)Char.GetNumericValue(oponentResponses[i][3]));
                
                //if the response move ends on a king 
                if(BoardManager.pieces[endSquareIndex].piece == Char.ToLower(Piece.KING))
                {
                    //if the king is the oponents king then the original move was not legal
                    if(BoardManager.pieces[endSquareIndex].color != BoardManager.currentTurnColor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static int getEndFileNumber(int index)
        {
            int fileNumber;

            fileNumber = (index / 8);
            fileNumber = BoardManager.correctFile(fileNumber);

            return fileNumber;
        }

        //transforms the position of the piece from index to short algebraic (e1 etc)
        public static string indexPositionToCoordinate(int indexPos)
        {
            string pos = "";
            int column;
            int rank;

            column = indexPos % 8;
            rank = (indexPos / 8);
            rank = BoardManager.correctFile(rank);

            pos += columnNumberToLetter(column);
            pos += rank;

            return pos;
        }

        private static char columnNumberToLetter(int column)
        {
            char c;

            switch(column)
            {
                case 0:
                    c = 'a';
                    break;
                case 1:
                    c = 'b';
                    break;
                case 2:
                    c = 'c';
                    break;
                case 3:
                    c = 'd';
                    break;
                case 4:
                    c = 'e';
                    break;
                case 5:
                    c = 'f';
                    break;
                case 6:
                    c = 'g';
                    break;
                case 7:
                    c = 'h';
                    break;
                default:
                    c = ' ';
                    System.Console.WriteLine(ErrorLogs.NONVALIDMOVEGENERATED);
                    Environment.Exit(1);
                    break;
            }
            return c;
        }
    }
}