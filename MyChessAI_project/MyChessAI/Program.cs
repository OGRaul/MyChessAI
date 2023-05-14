﻿using System;
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
            //FenParser.loadPositionFromFen(FenStrings.CASTLINGTESTINGFEN); //Loads castling testing board set up
            FenParser.loadPositionFromFen(FenStrings.CASTLINGBLOCKEDTESTINGFEN); //Loads castling BLOCKED testing board set up

            //FenParser.loadPositionFromFen("rnb1kbnr/ppp1pppp/3q4/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 1"); //Loads some other testing board set up

            MenuManager.showMenu();
        }
    }
}
