using Moves;
using Util;

namespace Board
{
    public static class BoardManager
    {           
        public static bool isKingInCheck;
        public static char? promotion; //example: e7e8q (pawn moves to e8 an promotes to queen)

        public static bool canWhiteCastleKingside;
        public static bool canWhiteCastleQueenside;
        public static bool canBlackCastleKingside;
        public static bool canBlackCastleQueenside;
        public static int castlingRookOffset = 0;
        
        public static bool isEnPassantMove;
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
            isKingInCheck = false;
            canWhiteCastleKingside = false;
            canWhiteCastleQueenside = false;
            canBlackCastleKingside = false;
            canBlackCastleQueenside = false;
            castlingRookOffset = 0;

            //resets en passant square and last piece moved
            isEnPassantMove = false;
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

        private static void printExtraFenData()
        {
            System.Console.WriteLine("---------------------------------");
            System.Console.WriteLine("DEBUG INFO: ");
            
            string s = currentTurn == Piece.WHITE ? "White" : "Black";
            System.Console.WriteLine("Color of current turn: "+s);
            
            checkCastlingRights();
            System.Console.WriteLine("canWhiteCastleKingside: "+canWhiteCastleKingside);
            System.Console.WriteLine("canWhiteCastleQueenside: "+canWhiteCastleQueenside);
            System.Console.WriteLine("canBlackCastleKingside: "+canBlackCastleKingside);
            System.Console.WriteLine("canBlackCastleQueenside: "+canBlackCastleQueenside);

            checkEnPassantSquare();
            System.Console.WriteLine("en passant square position: "+enPassantSquare?.position);
            
            System.Console.WriteLine("Turn count: "+turnCount);

            System.Console.WriteLine("---------------------------------");
        }

        public static string makeMove(string move)
        {
            //resets move specific variables
            promotion = null;
            isEnPassantMove = false;
            castlingRookOffset = 0;
            isKingInCheck = false;

            //TODO: is this efficient?
            //checks before each move who can castle and where and where en passant is possible
            checkCastlingRights();
            checkEnPassantSquare();

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

            if(isLegalMove(pieceToMove, placeToMove, endFile)) //added endFile to check for promotions and en passant
            {
                //handles castling moves
                if(castlingRookOffset != 0) //if its a castling move
                {
                    //leaves previous square empty
                    pieces[pieceToMove] = new Square();
                    pieces[pieceToMove].position = pieceToMove;
                    pieces[pieceToMove].hasMoved = true;
                    
                    //places the piece on its new square, replacing what was in it previously
                    pieces[placeToMove].piece = piece;
                    pieces[placeToMove].hasMoved = true;
                    int newKingSquare = placeToMove;

                    moveTheRook(newKingSquare);
                }
                //handles en passant moves
                else if(isEnPassantMove)
                {
                    //leaves previous square empty
                    pieces[pieceToMove] = new Square();
                    pieces[pieceToMove].position = pieceToMove;
                    pieces[pieceToMove].hasMoved = true;

                    int offset = currentTurn == Piece.WHITE ? PositionOffsets.upOne : PositionOffsets.downOne;

                    //leaves square of en passanted piece empty
                    pieces[placeToMove+offset] = new Square();
                    pieces[placeToMove+offset].position = pieceToMove;
                    pieces[placeToMove+offset].hasMoved = true;

                    //places the pawn on its new square
                    pieces[placeToMove].piece = piece;
                    pieces[placeToMove].hasMoved = true;
                }
                //handles regular moves
                else
                {
                    //leaves previous square empty
                    pieces[pieceToMove] = new Square();
                    pieces[pieceToMove].position = pieceToMove;
                    pieces[pieceToMove].hasMoved = true;
                    
                    //places the piece on its new square, replacing what was in it previously
                    pieces[placeToMove].piece = piece;
                    pieces[placeToMove].hasMoved = true;
                }

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

            //makes sure the enPassant square is back to invalid
            enPassantSquare = null;

            //toggles the turn
            toggleTurn();
            turnCount++;

            return moveMade;
        }

        //moves the rook to its new position in castling
        private static void moveTheRook(int newKingSquare)
        {
            //places the rook on its new correct position
            if(castlingRookOffset == 1) //queen side
            {
                //if it is white
                if(currentTurn == Piece.WHITE)
                {
                    //resets the square where the left rook was
                    int oldpos = pieces[PiecePositions.leftWhiteRookPosition].position;
                    pieces[PiecePositions.leftWhiteRookPosition] = new Square();
                    pieces[PiecePositions.leftWhiteRookPosition].position = oldpos;
                    pieces[PiecePositions.leftWhiteRookPosition].hasMoved = true;

                    //sets the new square of the left rook
                    int placeToMoveRook = newKingSquare+castlingRookOffset;
                    pieces[placeToMoveRook].piece = Piece.ROOK;
                    pieces[placeToMoveRook].color = Piece.WHITE;
                    pieces[placeToMoveRook].hasMoved = true;
                }
                //if it is black
                if(currentTurn == Piece.BLACK)
                {
                    //resets the square where the left rook was
                    int oldpos = pieces[PiecePositions.leftBlackRookPosition].position;
                    pieces[PiecePositions.leftBlackRookPosition] = new Square();
                    pieces[PiecePositions.leftBlackRookPosition].position = oldpos;
                    pieces[PiecePositions.leftBlackRookPosition].hasMoved = true;

                    //sets the new square of the left rook
                    int placeToMoveRook = newKingSquare+castlingRookOffset;
                    pieces[placeToMoveRook].piece = Piece.ROOK;
                    pieces[placeToMoveRook].color = Piece.BLACK;
                    pieces[placeToMoveRook].hasMoved = true;
                }
            }
            else if(castlingRookOffset == -1)
            {
                    //if it is white
                if(currentTurn == Piece.WHITE)
                {
                    //resets the square where the left rook was
                    int oldpos = pieces[PiecePositions.rightWhiteRookPosition].position;
                    pieces[PiecePositions.rightWhiteRookPosition] = new Square();
                    pieces[PiecePositions.rightWhiteRookPosition].position = oldpos;
                    pieces[PiecePositions.rightWhiteRookPosition].hasMoved = true;

                    //sets the new square of the left rook
                    int placeToMoveRook = newKingSquare+castlingRookOffset;
                    pieces[placeToMoveRook].piece = Piece.ROOK;
                    pieces[placeToMoveRook].color = Piece.WHITE;
                    pieces[placeToMoveRook].hasMoved = true;
                }
                //if it is black
                if(currentTurn == Piece.BLACK)
                {
                    //resets the square where the left rook was
                    int oldpos = pieces[PiecePositions.rightBlackRookPosition].position;
                    pieces[PiecePositions.rightBlackRookPosition] = new Square();
                    pieces[PiecePositions.rightBlackRookPosition].position = oldpos;
                    pieces[PiecePositions.rightBlackRookPosition].hasMoved = true;

                    //sets the new square of the left rook
                    int placeToMoveRook = newKingSquare+castlingRookOffset;
                    pieces[placeToMoveRook].piece = Piece.ROOK;
                    pieces[placeToMoveRook].color = Piece.BLACK;
                    pieces[placeToMoveRook].hasMoved = true;
                }
            }
        }

        //gets the index position in the array of squares corresponding to the given algebraic position
        public static int coordinateToIndexPosition(int columnNumber, int fileNumber)
        {
            int square;

            //the file number needs to be corrected because the board is inversed
            fileNumber = correctFile(fileNumber);

            square = fileNumber * 8 + columnNumber-1;

            return square;
        }

        //converts a column letter to numeric value for use in algebraic positioning
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

        //checks castling rights for both colors
        public static void checkCastlingRights()
        {
            canWhiteCastleQueenside = canWhiteQueenSideCastle();
            canWhiteCastleKingside = canWhiteKingSideCastle();

            canBlackCastleQueenside = canBlackQueenSideCastle();
            canBlackCastleKingside = canBlackKingSideCastle();
        }

        //checks if enpassant is possible and if it is where is it
        public static void checkEnPassantSquare()
        {
            //if the last piece moved was a pawn checks to the side of the pawns
            if(lastPieceMoved != null && Char.ToLower(lastPieceMoved.piece) == Piece.PAWN)
            {
                //if the turn is white the enPassantRank is the fifth, otherwise its the fourth
                int[] enPassantRank = currentTurn == Piece.WHITE ? Ranks.fifthRank : Ranks.fourthRank;

                System.Console.WriteLine(enPassantRank.First());

                if(isPieceInRank(lastPieceMoved, enPassantRank))
                {
                    int positionRightOfPawnMoved = lastPieceMoved.position + PositionOffsets.rightOne;
                    int positionLeftOfPawnMoved = lastPieceMoved.position + PositionOffsets.leftOne;
                    
                    Square? squareRightOfPawnMoved = null;
                    Square? squareLeftOfPawnMoved = null;
                    
                    //checks if the square right from the last piece moved is in the same rank
                    if(isItTheSameRank(lastPieceMoved.position, positionRightOfPawnMoved, enPassantRank))
                    {
                        squareRightOfPawnMoved = pieces[positionRightOfPawnMoved];
                    }

                    //checks if the square left from the last piece moved is in the same rank
                    if(isItTheSameRank(lastPieceMoved.position, positionLeftOfPawnMoved, enPassantRank))
                    {
                        squareLeftOfPawnMoved = pieces[positionLeftOfPawnMoved];
                    }

                    //the square above the last pawn moved
                    int upOfPawnMoved = currentTurn == Piece.WHITE ? lastPieceMoved.position + PositionOffsets.downOne : lastPieceMoved.position + PositionOffsets.upOne;
                    
                    //if the square right of the last pawn moved by oponent is a pawn of the current turns color
                    if(squareRightOfPawnMoved != null && Char.ToLower(squareRightOfPawnMoved.piece) == Piece.PAWN && squareRightOfPawnMoved.color == currentTurn)
                    {
                        //the en passant square is set to the squre right above the last moved pawn
                        enPassantSquare = pieces[upOfPawnMoved];
                    }
                    //else if the square right of the last pawn moved by oponent is a pawn of the current turns color
                    else if(squareLeftOfPawnMoved != null && Char.ToLower(squareLeftOfPawnMoved.piece) == Piece.PAWN && squareLeftOfPawnMoved.color == currentTurn)
                    {
                        //the en passant square is set to the squre right above the last moved pawn
                        enPassantSquare = pieces[upOfPawnMoved];
                    }
                    else
                    {
                        enPassantSquare = null;
                    }
                }
            }
        }

        private static bool isPieceInRank(Square piece, int[] rank)
        {
            for(int i = 0; i < rank.Length; i++)
            {
                if(piece.position == rank[i])
                {
                    return true;
                }
            }

            return false;
        }

        private static bool canWhiteQueenSideCastle()
        {
            bool canCastle = true;

            //checks movement of white king and rooks
            if(pieces[PiecePositions.whiteKingPosition].hasMoved)
            {
                canCastle = false;
            }
            //has the white queen side rook moved yet?
            else if(pieces[PiecePositions.leftWhiteRookPosition].hasMoved)
            {
                canCastle = false;
            }

            return canCastle;
        }

        private static bool isItTheSameRank(int firstPos, int secondPos, int[] rank)
        {
            bool isFirstPosInRank = false;
            bool isSecondPosInRank = false;

            for(int i = 0; i < 8; i++)
            {
                if(firstPos == rank[i])
                {
                    isFirstPosInRank = true;
                }

                if(secondPos == rank[i])
                {
                    isSecondPosInRank = true;
                }
            }

            return isFirstPosInRank && isSecondPosInRank;
        }

        private static bool canWhiteKingSideCastle()
        {
            bool canCastle = true;

            //checks movement of black king and rooks
            if(pieces[PiecePositions.whiteKingPosition].hasMoved)
            {
                canCastle = false;
            }
            //has the white queen side rook moved yet?
            else if(pieces[PiecePositions.rightWhiteRookPosition].hasMoved)
            {
                canCastle = false;
            }

            return canCastle;
        }

        private static bool canBlackQueenSideCastle()
        {
            bool canCastle = true;

            //checks movement of black king and rooks
            if(pieces[PiecePositions.blackKingPosition].hasMoved)
            {
                canCastle = false;
            }
            //has the white queen side rook moved yet?
            else if(pieces[PiecePositions.leftBlackRookPosition].hasMoved)
            {
                canCastle = false;
            }

            return canCastle;
        }

        private static bool canBlackKingSideCastle()
        {
            bool canCastle = true;

            //checks movement of black king and rooks
            if(pieces[PiecePositions.blackKingPosition].hasMoved)
            {
                canCastle = false;
            }
            //has the white queen side rook moved yet?
            else if(pieces[PiecePositions.rightBlackRookPosition].hasMoved)
            {
                canCastle = false;
            }

            return canCastle;
        }

        //changes whos turn it is
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

        /*
            since the chessboard first square is not the 1-1 algebraic position,
            due to it being fliped, this corrects the file to the point where it should be
        */
        public static int correctFile(int file)
        {
            file = (file *-1) + 8;
            return file;
        }

        /*
            Starts checking if its a legal move. This tests basic things that are needed
            for any kind of move to be legal. If it passes, it sends the move further down
            to check if its legal for the specific piece in the specific position.
        */
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

        //divides the move by what piece is being move to check legality
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

        //checks for legal pawn moves
        public static bool isValidPawnMove(Square movingPiece, Square endSquare)
        {
            int offset = endSquare.position - movingPiece.position; 

            //if a pawn moves to the 7th or first rank and there is no promotion its not legal
            if(promotion == null)
            {
                List<int> promotionSquares = new List<int>();
                promotionSquares.AddRange(Ranks.allRanks[0]);
                promotionSquares.AddRange(Ranks.allRanks[7]);
                
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
            //if its a diagonal move and theres not an enemy piece there its forbidden too (unless en passant is possible in that square)
            else if (offset == PositionOffsets.upLeft || offset == PositionOffsets.upRight) 
            {
                //if the diagonal square is emtpy checks for en passant
                if(endSquare.color == Piece.COLORNONE)
                {   
                    //if en passant is not possible in that square it is ilegal 
                    if(enPassantSquare == null)
                    {
                        return false;
                    }
                    else if(endSquare.position != enPassantSquare.position)
                    {
                        return false;
                    }
                    else if(endSquare.position == enPassantSquare.position)
                    {
                        isEnPassantMove = true;
                    }
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

        //checks for valid knight moves
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

        //checks for valid bishop moves
        public static bool isValidBishopMove(Square movingPiece, Square endSquare)
        {
            return isValidSlidingPieceMove(movingPiece, endSquare, movingPiece.piece);
        }

        //checks for valid rook moves
        public static bool isValidRookMove(Square movingPiece, Square endSquare)
        {
            return isValidSlidingPieceMove(movingPiece, endSquare, movingPiece.piece);
        }

        //checks for valid queen moves
        public static bool isValidQueenMove(Square movingPiece, Square endSquare)
        {
            return isValidSlidingPieceMove(movingPiece, endSquare, movingPiece.piece);
        }

        //handles the sliding pieces movement in one method due to how similar they all move
        public static bool isValidSlidingPieceMove(Square movingPiece, Square endSquare, char pieceType)
        {
            //on the base of what piece it is checks only those directions
            int[]? posibleDirections = null;

            //checks for what piece it is and what directions that piece can legaly move to
            if(char.ToLower(pieceType) == Piece.QUEEN)
            {
                posibleDirections = PositionOffsets.allDirections;
            }
            else if(char.ToLower(pieceType)  == Piece.BISHOP)
            {
                posibleDirections = PositionOffsets.diagonals;
            }
            else if(char.ToLower(pieceType)  == Piece.ROOK)
            {
                posibleDirections = PositionOffsets.vertHorz;
            }

            //if its none of the above pieces, then there was a mistake somewhere
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

        //checks for valid king moves
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

            //if the move is two squares to the left or right its castling. this checks if its legal
            if(offset == PositionOffsets.rightOne*2 || offset == PositionOffsets.leftOne*2)
            {
                //if the king is in check its not legal
                if(isKingInCheck)
                {
                    return false;
                }

                //if it would collide with another piece its not legal
                if(offset == PositionOffsets.rightOne*2)
                {
                    //if it would encounter another piece on its way or the king would be in check
                    if(pieces[movingPiece.position+PositionOffsets.rightOne].piece != Piece.PIECENONE || revealsCheck(movingPiece, pieces[movingPiece.position+PositionOffsets.rightOne]))
                    {
                        return false;
                    }
                }
                else if(offset == PositionOffsets.leftOne*2)
                {
                    //if it would encounter another piece on its way or the king would be in check
                    if(pieces[movingPiece.position+PositionOffsets.leftOne].piece != Piece.PIECENONE || revealsCheck(movingPiece, pieces[movingPiece.position+PositionOffsets.leftOne]))
                    {
                        return false;
                    } 
                    //for queen side castling it also needs to check if there is something beside the rook
                    else if(pieces[movingPiece.position+PositionOffsets.leftOne*3].piece != Piece.PIECENONE)
                    {
                        return false;
                    }
                }


                //if its whites turn it returns castling rights queen and king side of white depending on direction of move
                if(currentTurn == Piece.WHITE)
                {
                    if(offset == PositionOffsets.rightOne*2)
                    {
                        if(canWhiteCastleKingside)
                        {
                            castlingRookOffset = -1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if(offset == PositionOffsets.leftOne*2)
                    {
                        if(canWhiteCastleQueenside)
                        {
                            castlingRookOffset = 1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                //if its blacks turn it returns castling rights queen and king side of black depending on direction of move
                else if(currentTurn == Piece.BLACK)
                {
                     if(offset == PositionOffsets.rightOne*2)
                    {
                        if(canBlackCastleKingside)
                        {
                            castlingRookOffset = -1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if(offset == PositionOffsets.leftOne*2)
                    {
                        if(canBlackCastleQueenside)
                        {
                            castlingRookOffset = +1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        //tests for checks to not make it possible to put your own king in check
        public static bool revealsCheck(Square movingPiece, Square endSquare)
        {
            //TODO: look for checks
            
            //TODO: look to see if the king is already in check and if it is save it here
            //isKingInCheck = true;

            return false;
        }
    }
}