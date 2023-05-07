using System;
using Moves;
using Resources;

namespace Board
{
    public static class FenParser
    {
        //keeps location of the position on the board being handled by the loadPositionFromFen
        public static int currentPiece = 0;
        public const string STARTPOS = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static void loadPositionFromFen (string? fen)
        {   
            //restarts the board in default values
            BoardManager.restartBoardVariables();

            //if not specified starts normal beginning position of chess
            if(fen == null || fen == "" || fen == " ")
            {
                fen = STARTPOS;
            }

            /*
                Goes through the first part of the fen strings that determines 
                the pieces position on the board.

                The variable i stores the position on the fen string being handled
                while currentPiece stores the position on the board being handled
             */
            for (int i = 0; i < fen.Length; i++)
            {
                //if its a blank space it asumes al pieces are set and extra fen info is now getting parsed
                if(fen[i] == ' ')
                {
                    currentPiece = 0;
                    extraFenInfo(fen, i+1);
                    return;
                }

                //if its a number
                if(System.Char.IsDigit(fen[i]) && !fen[i].Equals("/"))
                {
                    int n = fen[i] - '0';
                    for(int j = 0; j < n; j++)
                    {
                        BoardManager.pieces[currentPiece] = new Square();
                        BoardManager.pieces[currentPiece].position = currentPiece;

                        BoardManager.pieces[currentPiece].piece =  Piece.PIECENONE;
                        BoardManager.pieces[currentPiece].color =  Piece.COLORNONE;

                        currentPiece++;
                    }
                }
                else if(!System.Char.IsDigit(fen[i]))
                {
                    BoardManager.pieces[currentPiece] = new Square();
                    BoardManager.pieces[currentPiece].position = currentPiece;

                    switch(fen[i])
                    {
                        case 'r':
                            BoardManager.pieces[currentPiece].piece =  Piece.ROOK;
                            BoardManager.pieces[currentPiece].color =  Piece.BLACK;
                        break;
                        case 'R':
                            BoardManager.pieces[currentPiece].piece =  Piece.ROOK;
                            BoardManager.pieces[currentPiece].color =  Piece.WHITE;
                        break;
                        case 'n':
                            BoardManager.pieces[currentPiece].piece =  Piece.KNIGHT;
                            BoardManager.pieces[currentPiece].color =  Piece.BLACK;
                        break;
                        case 'N':
                            BoardManager.pieces[currentPiece].piece =  Piece.KNIGHT;
                            BoardManager.pieces[currentPiece].color =  Piece.WHITE;
                        break;
                        case 'b':
                            BoardManager.pieces[currentPiece].piece =  Piece.BISHOP;
                            BoardManager.pieces[currentPiece].color =  Piece.BLACK;
                        break;
                        case 'B':
                            BoardManager.pieces[currentPiece].piece =  Piece.BISHOP;
                            BoardManager.pieces[currentPiece].color =  Piece.WHITE;
                        break;
                        case 'q':
                            BoardManager.pieces[currentPiece].piece =  Piece.QUEEN;
                            BoardManager.pieces[currentPiece].color =  Piece.BLACK;
                        break;
                        case 'Q':
                            BoardManager.pieces[currentPiece].piece =  Piece.QUEEN;
                            BoardManager.pieces[currentPiece].color =  Piece.WHITE;
                        break;
                        case 'k':
                            BoardManager.pieces[currentPiece].piece =  Piece.KING;
                            BoardManager.pieces[currentPiece].color =  Piece.BLACK;
                        break;
                        case 'K':
                            BoardManager.pieces[currentPiece].piece =  Piece.KING;
                            BoardManager.pieces[currentPiece].color =  Piece.WHITE;
                        break;
                        case 'p':
                            BoardManager.pieces[currentPiece].piece =  Piece.PAWN;
                            BoardManager.pieces[currentPiece].color =  Piece.BLACK;
                        break;
                        case 'P':
                            BoardManager.pieces[currentPiece].piece =  Piece.PAWN;
                            BoardManager.pieces[currentPiece].color =  Piece.WHITE;
                            break;
                        case '/':
                            currentPiece--;
                            break;
                        default:
                            throwFenError(ErrorLogs.MAINPARTOFFEN+ErrorLogs.NONVALIDCHAR);
                            currentPiece--;
                            break;
                    }
                    currentPiece++;
                }
            }

            currentPiece = 0;
        }

        /*
            Interpretates the extra part of the fen, like: " w KQkq - 0 1" 
            Its the turn, castling rights, where is enpassant possible 
            how many turns without captures or pawn pushes(for 50 move rule) and the turn count

            This first part checks the turn part and sends the rest down to be handled by
            other methods, always on the next space char
        */
        private static void extraFenInfo(string fen, int startOfExtra)
        {
            //looks for whos turn it is
            for (int i = startOfExtra; i < fen.Length; i++)
            {
                switch(fen[i])
                {
                    case ' ':
                        extraFenInfoCastling(fen, i+1);
                        return;
                    case 'w':
                        BoardManager.currentTurn = Piece.WHITE;
                        break;
                    case 'W':
                        BoardManager.currentTurn = Piece.WHITE;
                        break;
                    case 'b':
                        BoardManager.currentTurn = Piece.BLACK;
                        break;
                    case 'B':
                        BoardManager.currentTurn = Piece.BLACK;
                        break;
                    default:
                        throwFenError(ErrorLogs.TURNPARTOFFEN+ErrorLogs.NONVALIDCHAR);
                        break;
                }
            }
        }

        //handles the castling part of the fen string
        private static void extraFenInfoCastling(string fen, int startOfExtra)
        {
            //looks for who can castle and where
            for (int i = startOfExtra; i < fen.Length; i++)
            {
                switch(fen[i])
                {
                    case ' ':
                        extraFenInfoEnPassant(fen, i+1);
                        return;
                    case 'K':
                        BoardManager.canWhiteCastleKingside = true;
                        break;
                    case 'Q': 
                        BoardManager.canWhiteCastleQueenside = true;
                        break;
                    case 'k':
                        BoardManager.canBlackCastleKingside = true;
                        break;
                    case 'q':
                        BoardManager.canBlackCastleQueenside = true;
                        break;
                    //no one can castle 
                    case '-':
                        BoardManager.canWhiteCastleKingside = false;
                        BoardManager.canWhiteCastleQueenside = false;
                        BoardManager.canBlackCastleKingside = false;
                        BoardManager.canBlackCastleQueenside = false;
                        extraFenInfoEnPassant(fen, i+2);
                        return;
                    default:
                        throwFenError(ErrorLogs.CASTLINGPARTOFFEN+ErrorLogs.NONVALIDCHAR);
                        break;
                }
            }
        }

        //handles the en passant part of the fen string
        private static void extraFenInfoEnPassant(string fen, int startOfExtra)
        {
            //gets the en passant part of the fen string as a separate string
            string enPassantPartOfFen = "";

            for (int i = startOfExtra; i < fen.Length; i++)
            {
                //when it finds a whitespace the en passant part of the fen string is done
                if(fen[i] == ' ')
                {
                    break;
                }
                else
                {
                    enPassantPartOfFen += fen[i];
                }
            }

            if(enPassantPartOfFen.Length == 0)
            {
                throwFenError(ErrorLogs.ENPASSANTPARTOFFEN+ErrorLogs.TOOLONGORTOOSHORT);
            }
            if(enPassantPartOfFen.Length > 2)
            {
                throwFenError(ErrorLogs.ENPASSANTPARTOFFEN+ErrorLogs.TOOLONGORTOOSHORT);
            }

            //if the first character is not one of the selected ones then there is an error
            if(!isCharPartOfAString(enPassantPartOfFen[0], "-abcdefgh"))
            {
                throwFenError(ErrorLogs.ENPASSANTPARTOFFEN+ErrorLogs.NONVALIDCHAR);
            }
            //if the first character is a - and there is a second character there is an error
            if(enPassantPartOfFen[0] == '-' && enPassantPartOfFen.Length != 1)
            {
                throwFenError(ErrorLogs.ENPASSANTPARTOFFEN+ErrorLogs.NONVALIDCHAR);
            }
            //if the second character is not 1234567 or 8 there is an error
            if(enPassantPartOfFen.Length == 2 && !isCharPartOfAString(enPassantPartOfFen[1], "12345678"))
            {
                throwFenError(ErrorLogs.ENPASSANTPARTOFFEN+ErrorLogs.NONVALIDCHAR);
            }

            if(enPassantPartOfFen.Length == 1)
            {
                BoardManager.enPassantSquare = null;
            }
            else if(enPassantPartOfFen.Length == 2)
            {
                int column = BoardManager.columnLetterToNumber(enPassantPartOfFen[0]);
                int rank = (int) Char.GetNumericValue(enPassantPartOfFen[1]);
                int square = BoardManager.coordinateToIndexPosition(column, rank);
                BoardManager.enPassantSquare = BoardManager.pieces[square];
                
                //if the supposed to be possible en passant square is not empty it corrects the fen string to en passant not being possible
                if(BoardManager.enPassantSquare.piece != Piece.PIECENONE)
                {
                    enPassantPartOfFen = "-";
                    BoardManager.enPassantSquare = null;
                }
            }
            
            //manages the fifty move rule setting
            extraFenInfoFiftyMoveRulePart(fen, startOfExtra+enPassantPartOfFen.Length+1);
        }

        //handles the 50 move rule part of the fen string
        private static void extraFenInfoFiftyMoveRulePart(string fen, int startOfExtra)
        {
            //gets the Fifty Move Rule part of the fen string as a separate string
            string fiftyMoveRulePartOfFen = "";

            for (int i = startOfExtra; i < fen.Length; i++)
            {
                //when it finds a whitespace the Fifty Move Rule part of the fen string is done
                if(fen[i] == ' ')
                {
                    break;
                }
                else
                {
                    fiftyMoveRulePartOfFen += fen[i];
                }
            }

            if(fiftyMoveRulePartOfFen.Length == 0 || fiftyMoveRulePartOfFen.Length > 2)
            {
                throwFenError(ErrorLogs.FIFTYMOVERULEPARTOFFEN+ErrorLogs.TOOLONGORTOOSHORT);
            }

            if(fiftyMoveRulePartOfFen.Length == 1 && !isCharPartOfAString(fiftyMoveRulePartOfFen[0], "0123456789"))
            {
                throwFenError(ErrorLogs.FIFTYMOVERULEPARTOFFEN+ErrorLogs.NONVALIDCHAR);
            }

            if(fiftyMoveRulePartOfFen.Length == 2)
            {
                if(!isCharPartOfAString(fiftyMoveRulePartOfFen[0], "0123456789") || !isCharPartOfAString(fiftyMoveRulePartOfFen[1], "0123456789"))
                {
                    throwFenError(ErrorLogs.FIFTYMOVERULEPARTOFFEN+ErrorLogs.NONVALIDCHAR);
                }
            }

            //if none of the tests above throw a fen error then its valid
            BoardManager.movesSinceLastProgress = Int32.Parse(fiftyMoveRulePartOfFen);

            //TODO: add the turn counter support for the fen string
            extraFenInfoTurnCountPart(fen, startOfExtra+fiftyMoveRulePartOfFen.Length+1);
        }

        //handles the turn count part of the fen string
        private static void extraFenInfoTurnCountPart(string fen, int startOfExtra)
        {
            //gets the position after the fifty moves part to the end of the fen string
            string TurnCountPartOfFen = "";

            for (int i = startOfExtra; i < fen.Length; i++)
            {
                //when it finds a whitespace it ignores it
                if(fen[i] == ' ')
                {
                    //ignores white spaces after the fen string
                }
                else if(!isCharPartOfAString(fen[i], "0123456789 "))
                {
                    throwFenError(ErrorLogs.TURNCOUNTPARTOFFEN+ErrorLogs.NONVALIDCHAR);
                }
                else
                {
                    TurnCountPartOfFen += fen[i];
                }
            }
            
            int turnCountValue = Int32.Parse(TurnCountPartOfFen);

            if(turnCountValue > 1000 || turnCountValue < 1)
            {
                throwFenError(ErrorLogs.TURNCOUNTPARTOFFEN+ErrorLogs.TOOLONGORTOOSHORT);
            }
            else
            {
                BoardManager.turnCount = turnCountValue;
            }
        }

        //if the fen string is invalid this gets thrown to let the player know what part is wrong
        private static void throwFenError(string motive)
        {
            System.Console.WriteLine(ErrorLogs.INVALIDFEN);
            System.Console.WriteLine(motive);
            throw new FormatException(); 
        }

        //tests if a given char is located somewhere within a given string
        private static bool isCharPartOfAString(char charToTest, string validChars)
        {
            bool valid = false;
            for(int i = 0; i < validChars.Length; i++)
            {
                if(charToTest == validChars[i])
                {
                    valid = true;
                }
            }
            return valid;
        }

        //makes a fen string based on the current position on the board
        public static string loadFenFromPosition()
        {
            string newFen = "";

            //TODO: create loadFenFromPosition method
            

            return newFen;
        }
    }
}