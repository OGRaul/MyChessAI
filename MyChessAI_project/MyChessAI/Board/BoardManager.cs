using Moves;
using Resources;

namespace Board
{
    public static class BoardManager
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
        public static int[][] ranks = 
        {
            firstRank, secondRank, thirdRank, fourthRank,
            fifthRank, sixthRank, seventhRank, eigthRank
        };
                
        public static char? promotion; //example: e7e8q (pawn moves to e8 an promotes to queen)

        //position info
        public static bool canWhiteCastleKingside;
        public static bool canWhiteCastleQueenside;
        public static bool canBlackCastleKingside;
        public static bool canBlackCastleQueenside;
        public static Square? enPassantSquare;
        public static Square? lastPieceMoved;        
        public static int currentTurn;
        public static int turnCount = 1;
        public static int movesSinceLastProgress = 0;
        public static string moveHistory = "";
        public static Square[] pieces = new Square[64];

        public static void restartBoardVariables()
        {
            //resets castling rights
            canWhiteCastleKingside = false;
            canWhiteCastleQueenside = false;
            canBlackCastleKingside = false;
            canBlackCastleQueenside = false;;

            //resets en passant square and last piece moved
            enPassantSquare = null;
            lastPieceMoved = null;

            //defaults the turn to white
            currentTurn = Piece.WHITE;

            //turn count back to first
            turnCount = 1;

            //resets 50 move rule
            movesSinceLastProgress = 0;

            //empties previous move history
            moveHistory = "";

            //resets all squares
            pieces = new Square[64];
        }

        public static void drawBoard()
        {
            //TODO: for debug
            printExtraFenData();

            int files = 8;
            System.Console.WriteLine("Board:");
            System.Console.WriteLine("---------------");
            for (int i = 0; i < 64; i++)
            {
                if(i > 1 && i % 8 == 0)
                {
                    Console.Write("| "+files);
                    System.Console.WriteLine();
                    files--;
                }
                System.Console.Write(pieces[i].piece+" ");
            }
            Console.Write("| 1");
            System.Console.WriteLine();
            System.Console.WriteLine("_ _ _ _ _ _ _ _");
            System.Console.WriteLine("a b c d e f g h");
        }

        //TODO: for debug
        private static void printExtraFenData()
        {
            System.Console.WriteLine("---------------------------------");
            System.Console.WriteLine("DEBUG INFO: ");
            
            System.Console.WriteLine("Color of current turn(8 white, 16 black): "+currentTurn);
            
            System.Console.WriteLine("canWhiteCastleKingside: "+canWhiteCastleKingside);
            System.Console.WriteLine("canWhiteCastleQueenside: "+canWhiteCastleQueenside);
            System.Console.WriteLine("canBlackCastleKingside: "+canBlackCastleKingside);
            System.Console.WriteLine("canBlackCastleQueenside: "+canBlackCastleQueenside);

            System.Console.WriteLine("en passant square position: "+enPassantSquare?.position);
            
            System.Console.WriteLine("Turn count: "+turnCount);

            System.Console.WriteLine("---------------------------------");
        }

        public static string makeMove(string move)
        {
            promotion = null;

            int startColumn, startFile, endColumn, endFile; //example: e2e4

            startColumn = columnLetterToNumber(move[0]);
            endColumn = columnLetterToNumber(move[2]);

            if(startColumn == -1 || endColumn == -1)
            {
                System.Console.WriteLine(ErrorLogs.NONVALIDMOVE);
                return "0000";
            }

            if(move.Length == 5)
            {

                switch(move[4])
                {
                    case 'q':
                        promotion = Piece.QUEEN;
                        break;
                    case 'Q':
                        promotion = Piece.QUEEN;
                        break;
                    case 'n':
                        promotion = Piece.KNIGHT;
                        break;
                    case 'N':
                        promotion = Piece.KNIGHT;
                        break;
                    case 'b':
                        promotion = Piece.BISHOP;
                        break;
                    case 'B':
                        promotion = Piece.BISHOP;
                        break;
                    case 'r':
                        promotion = Piece.ROOK;
                        break;
                    case 'R':
                        promotion = Piece.ROOK;
                    break;
                    case ' ':
                        promotion = null;
                    break;
                     case '\x0000':
                        promotion = null;
                    break;
                    default:
                        System.Console.WriteLine(ErrorLogs.NONVALIDMOVE);
                        return "0000";
                }
            }
            
            //handle move[1] move[3]
            startFile = (int)Char.GetNumericValue(move[1]);
            endFile = (int)Char.GetNumericValue(move[3]);

            //combine the moves to a postion square
            int pieceToMove = coordinateToIndexPosition(startColumn, startFile);
            int placeToMove = coordinateToIndexPosition(endColumn, endFile);
            
            //saves the piece being moved
            char piece = pieces[pieceToMove].piece;

            if(isLegalMove(pieceToMove, placeToMove, endFile)) //add endcolumn to check for promotions and en passant
            {
                //leaves previous square empty
                pieces[pieceToMove] = new Square();
                pieces[pieceToMove].position = pieceToMove;
                
                //places the piece on its new square, replacing what was in it previously
                pieces[placeToMove].piece = piece;
                pieces[placeToMove].hasMoved = true;

                if(Char.IsUpper(piece))
                {
                    pieces[placeToMove].color = Piece.WHITE;
                }
                else
                {
                    pieces[placeToMove].color = Piece.BLACK;
                }

                //saves the last piece moved
                lastPieceMoved = pieces[placeToMove];
            }
            else
            {
                System.Console.WriteLine(ErrorLogs.ILEGALMOVE);
                return "0000";
            }

            //after a move is made if there is a promotion it executes it
            //white promotion
            if(promotion != null && currentTurn == Piece.WHITE) 
            {
                promotion = char.ToUpper((char) promotion);
                pieces[placeToMove].piece = (char) promotion;
            }
            //black promotions
            else if(promotion != null && currentTurn == Piece.BLACK) 
            {
                pieces[placeToMove].piece = (char) promotion;
            }

            //saves the move in numbers easily tested by humans
            string moveMade = 
                                startColumn.ToString()+
                                startFile.ToString()+
                                endColumn.ToString()+
                                endFile.ToString();
            
            System.Console.WriteLine(moveMade); //TODO: for debug

            //toggles the turn
            toggleTurn();
            turnCount++;

            return moveMade;
        }

        public static int coordinateToIndexPosition(int columnNumber, int fileNumber)
        {
            int square;

            //the file number needs to be corrected because the board is inversed
            fileNumber = correctFile(fileNumber);

            square = fileNumber * 8 + columnNumber-1;

            return square;
        }

        public static int columnLetterToNumber(char letter)
        {
            int columnNumber;

            switch(letter)
            {
                case 'a':
                    columnNumber = 1;
                    break;
                case 'b':
                    columnNumber = 2;
                    break;
                case 'c':
                    columnNumber = 3;
                    break;
                case 'd':
                    columnNumber = 4;
                    break;
                case 'e':
                    columnNumber = 5;
                    break;
                case 'f':
                    columnNumber = 6;
                    break;
                case 'g':
                    columnNumber = 7;
                    break;
                case 'h':
                    columnNumber = 8;
                    break;
                default:
                    columnNumber = -1;
                    System.Console.WriteLine(ErrorLogs.NONVALIDMOVE);
                    Environment.Exit(1);
                    break;
            }

            return columnNumber;
        }

        public static void toggleTurn()
        {
            if(currentTurn == Piece.WHITE)
            {
                currentTurn = Piece.BLACK;
            }
            else
            {
                currentTurn = Piece.WHITE;
            }
        }

        public static int correctFile(int file)
        {
            file = (file *-1) + 8;
            return file;
        }

        public static bool isLegalMove(int pieceToMove, int placeToMove, int endFile)
        {
            Square movingPiece = pieces[pieceToMove];
            Square endSquare = pieces[placeToMove];

            //checks that the player is not moving a piece of the wrong color
            if(movingPiece.color != currentTurn)
            {
                return false;
            }
            //checks that the player is not moving into his own piece
            if(endSquare.color == currentTurn)
            {
                return false;
            }

            //checks if the piece selected actually moves that way
            if(!isValidForThisPiece(movingPiece, endSquare))
            {
                return false;
            }

            //check if this move reveals a check on the currents player kings
            if(revealsCheck(movingPiece, endSquare))
            {
                return false;
            }

            return true;
        }

        public static bool isValidForThisPiece(Square movingPiece, Square endSquare)
        {
            switch(Char.ToLower(movingPiece.piece))
            {
                case Piece.PAWN:
                    return isValidPawnMove(movingPiece, endSquare);
                case Piece.BISHOP:
                    return isValidBishopMove(movingPiece, endSquare);
                case Piece.ROOK:
                    return isValidRookMove(movingPiece, endSquare);
                case Piece.KNIGHT:
                    return isValidKnightMove(movingPiece, endSquare);
                case Piece.QUEEN:
                    return isValidQueenMove(movingPiece, endSquare);
                case Piece.KING:
                    return isValidKingMove(movingPiece, endSquare);
                default:
                    System.Console.WriteLine(ErrorLogs.PIECEDOESNOTEXIST);
                    System.Environment.Exit(0); 
                    return false;
            }
        }

        public static bool isValidPawnMove(Square movingPiece, Square endSquare)
        {
            int offset = endSquare.position - movingPiece.position; 

            //if a pawn moves to the 7th or first rank and there is no promotion its not legal
            if(promotion == null)
            {
                List<int> promotionSquares = new List<int>();
                promotionSquares.AddRange(ranks[0]);
                promotionSquares.AddRange(ranks[7]);
                
                for(int i = 0; i < promotionSquares.Count; i++)
                {
                    if(promotionSquares[i] == endSquare.position)
                    {
                        return false;
                    }
                }
            }

            //inverts legal vertical moves for white
            if(movingPiece.color == Piece.WHITE)
            {
                offset = offset*-1;
            }

            //if its not a theoretically posible pawn moves stops right there
            if(offset != PositionOffsets.upOne &&
                offset != (PositionOffsets.upOne*2) &&
               offset != PositionOffsets.upLeft && 
               offset != PositionOffsets.upRight)
            {
                return false;
            }
            //forbids pawns that have moved from moving double squares
            else if(offset == PositionOffsets.upOne*2 &&
                     movingPiece.hasMoved == true)
            {
                return false;
            }
            //if its a diagonal move and theres not an enemy piece there its forbidden too
            else if (offset == PositionOffsets.upLeft || offset == PositionOffsets.upRight) 
            {
                //TODO: check for en passant
                if(endSquare.color == Piece.COLORNONE || endSquare.color == currentTurn)
                {
                    return false;
                }
            }
            //if it moves one square vertically and the square is not emtpy its also a legal move
            else if(offset == PositionOffsets.upOne && endSquare.color != Piece.COLORNONE)
            {
                return false;
            }
            //if it moves two squares vertically and the square is not emtpy its also a legal move
            else if(offset == PositionOffsets.upOne*2 && endSquare.color != Piece.COLORNONE)
            {
                return false;
            }
            //BLACKSPECIFIC: 
            else if(currentTurn == Piece.BLACK)
            {
                //if it moves two squares it makes sure it doesn't jump over anyone
                if(offset == PositionOffsets.upOne*2 && pieces[endSquare.position + (PositionOffsets.upOne *-1)].color != Piece.COLORNONE)
                {
                    return false;
                }       
            }
            //WHITE SPECIFIC: 
            else if(currentTurn == Piece.WHITE)
            {
                //if it moves two squares it makes sure it doesn't jump over anyone
                if(offset == PositionOffsets.upOne*2 && pieces[endSquare.position + (PositionOffsets.upOne)].color != Piece.COLORNONE)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isValidKnightMove(Square movingPiece, Square endSquare)
        {
            int offset = endSquare.position - movingPiece.position; 

            //if its one of the valid moves returns true
            for (int i = 0; i < PositionOffsets.horseDirections.Length; i++)
            {
                if(offset == PositionOffsets.horseDirections[i])
                {
                    return true;
                }
            }

            return false;
        }

        public static bool isValidBishopMove(Square movingPiece, Square endSquare)
        {
            return isValidSlidingPieceMove(movingPiece, endSquare, movingPiece.piece);
        }

        public static bool isValidRookMove(Square movingPiece, Square endSquare)
        {
            return isValidSlidingPieceMove(movingPiece, endSquare, movingPiece.piece);
        }

        public static bool isValidQueenMove(Square movingPiece, Square endSquare)
        {
            return isValidSlidingPieceMove(movingPiece, endSquare, movingPiece.piece);
        }

        public static bool isValidSlidingPieceMove(Square movingPiece, Square endSquare, char pieceType)
        {
            //on the base of what piece it is checks only those directions
            int[]? posibleDirections = null;

            if(char.ToLower(pieceType) == Piece.QUEEN)
            {
                posibleDirections = PositionOffsets.allDirections;
            }
            else 
            if(char.ToLower(pieceType)  == Piece.BISHOP)
            {
                posibleDirections = PositionOffsets.diagonals;
            }
            else 
            if(char.ToLower(pieceType)  == Piece.ROOK)
            {
                posibleDirections = PositionOffsets.vertHorz;
            }

            if(posibleDirections == null)
            {
                System.Console.WriteLine(ErrorLogs.INVALIDMOVEDIRECTION);
                System.Environment.Exit(0); 
            }

            int currentPos = movingPiece.position;

            //goes through all posible directions for the queen
            for(int i = 0; i < posibleDirections.Length; i++)
            {
                //resets the current position to be the original plus the 1 new moving direction
                currentPos = movingPiece.position + posibleDirections[i];

                //if the move itself is a simple king type move its valid
                if(endSquare.position == currentPos )
                {
                    return true;
                }

                //otherwise it goes through the current direction all the way to the edge of the board
                for(int j = 0; j < PositionOffsets.getNumSquaresToEdge(currentPos, posibleDirections[i]); j++)
                {                    
                    //if the square is not empty it stops and looks at the color
                    if(pieces[currentPos].color != Piece.COLORNONE)
                    {
                        //if the piece is trying to move into its own color it stops this for loop
                        if(pieces[currentPos].color == currentTurn)
                        {
                            break;
                        }
                    }
                    else
                    {
                        //moves to the next square in line
                        currentPos += posibleDirections[i];
                    }

                    if(currentPos == endSquare.position)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool isValidKingMove(Square movingPiece, Square endSquare)
        {
            int offset = endSquare.position - movingPiece.position; 

            //TODO: implement the attacked check or if handled form revealsCheck() remove this instead
            //checks if king would be in check in its new square
            if(currentTurn == Piece.BLACK)
            {
                if(endSquare.isAtackedByWhite)
                {
                    return false;
                }
            }
            else if ((currentTurn == Piece.WHITE))
            {
                if(endSquare.isAtackedByBlack)
                {
                    return false;
                }
            }

            //basic legal moves for the king
            for(int i = 0; i < PositionOffsets.allDirections.Length; i++)
            {
                if(offset == PositionOffsets.allDirections[i])
                {
                    return true;
                }
            }

            //TODO: handle castling
            if(currentTurn == Piece.WHITE)
            {
                
            }
            else if(currentTurn == Piece.BLACK)
            {

            }

            return false;
        }

        public static bool revealsCheck(Square movingPiece, Square endSquare)
        {
            //TODO: look for checks
            return false;
        }
    }
}