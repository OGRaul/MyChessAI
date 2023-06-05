using System;
using Menu;
using Board;
using Util;

namespace MyChessAI
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //FenParser.loadPositionFromFen(null); //Loads standard board set up

            //FenParser.loadPositionFromFen(FenStrings.PAWNTESTINGFEN); //Loads pawn testing board set up
            //FenParser.loadPositionFromFen(FenStrings.KNIGHTTESTINGFEN); //Loads knight testing board set up
            //FenParser.loadPositionFromFen(FenStrings.KINGTESTINGFEN); //Loads king testing board set up
            //FenParser.loadPositionFromFen(FenStrings.QUEENTESTINGFEN); //Loads queen testing board set up
            //FenParser.loadPositionFromFen(FenStrings.BISHOPTESTINGFEN); //Loads bishop testing board set up
            //FenParser.loadPositionFromFen(FenStrings.ROOKTESTINGFEN); //Loads rook testing board set up
            //FenParser.loadPositionFromFen(FenStrings.PROMOTIONTESTINGFEN); //Loads promotion testing board set up
            //FenParser.loadPositionFromFen(FenStrings.ENPASSANTTESTINGFEN); //Loads en passant testing board set up
            //FenParser.loadPositionFromFen(FenStrings.ENPASSANTTESTINGFEN2); //Loads en passant 2 testing board set up            
            //FenParser.loadPositionFromFen(FenStrings.CASTLINGTESTINGFEN); //Loads castling testing board set up
            //FenParser.loadPositionFromFen(FenStrings.CASTLINGBLOCKEDTESTINGFEN); //Loads castling BLOCKED testing board set up

            FenParser.loadPositionFromFen("2bqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKB2 b Qk - 10 100"); //Loads some other testing board set up

            MenuManager.showMenu();
        }
    }
}
