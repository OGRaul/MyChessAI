using System;
using Board;
using Uci;
using Moves;
using Util;
using Engine;

namespace Menu
{
    public static class MenuManager
    {
        static MoveInterpreter moveInterpreter = new MoveInterpreter();
        
        public static void showMenu()
        {
            System.Console.WriteLine("Welcome to MyChessAI by ROG");

            while(true)
            {
                System.Console.WriteLine("=====================================================");
                System.Console.WriteLine("Type \"help\" for a list of available commands");

                string command = Console.ReadLine() ?? "";

                switch (command)
                {
                    case Commands.HELP:
                        listCommands();
                        break;
                    case Commands.SHOWBOARD:
                        BoardManager.drawBoard();
                        break;
                    case Commands.STARTAI:
                        System.Console.WriteLine("AI should play the next move"); //TODO: for debug
                        AI.makeEngineMove();
                        break;
                    case Commands.STARTUCI:
                        UciInterface.startUci();
                        break;
                    case Commands.ENTERFEN:
                        System.Console.WriteLine("Type a valid FEN and hit enter");
                        string? fen = Console.ReadLine();
                        FenParser.loadPositionFromFen(fen);
                        break;
                    case Commands.GETFEN:
                        System.Console.WriteLine(FenParser.createFenFromPosition()); 
                        break;
                    case Commands.EXIT:
                        System.Console.WriteLine("Closing...");
                        System.Environment.Exit(0); 
                        break;
                    default:
                        //if it looks like a move handle it like a move
                        if(moveInterpreter.isValidMove(command))
                        {
                            BoardManager.makeMove(command);
                        }
                        else
                        {
                            System.Console.WriteLine("Command: "+command+" is not a valid command");
                        }
                        break;
                }
            }
            
        }

        public static void listCommands()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Listing commands");
            System.Console.WriteLine("-------------------------------------------");
            
            Commands.listCommands();
        }
    }
}