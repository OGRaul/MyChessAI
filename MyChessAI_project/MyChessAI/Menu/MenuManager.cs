using System;
using Board;
using Uci;
using Moves;

namespace Menu
{
    public static class MenuManager
    {
        static MoveInterpreter moveInterpreter = new MoveInterpreter();
        public static void showMenu()
        {
            System.Console.WriteLine("Welcome to Chess AI by ROG");
            while(true)
            {
                System.Console.WriteLine("=====================================================");
                System.Console.WriteLine("Type \"help\" for a list of available commands");

                string command = Console.ReadLine() ?? "";

                switch (command)
                {
                    case Command.HELP:
                        listCommands();
                        break;
                    case Command.SHOWBOARD:
                        BoardManager.drawBoard();
                        break;
                    case Command.STARTAI:
                        System.Console.WriteLine("AI should play the next move"); //TODO: for debug
                        //TODO: start the ai here on the current move
                        break;
                    case Command.STARTUCI:
                        UciInterface.startUci();
                        break;
                    case Command.ENTERFEN:
                        System.Console.WriteLine("Type a valid FEN and hit enter");
                        string? fen = Console.ReadLine();
                        FenParser.loadPositionFromFen(fen);
                        break;
                    case Command.EXIT:
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
            
            Command.listCommands();
        }
    }
}