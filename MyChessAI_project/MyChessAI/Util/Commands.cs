using System;

namespace Util
{
    public class Commands
    {
        public const string HELP = "help";
        public const string MANHELP = "Lists all available commands and their definition";

        public const string SHOWBOARD = "d";
        public const string MANSHOWBOARD = "Displays the current board";

        public const string STARTAI = "go";
        public const string MANSTARTAI = "Starts the AI on the current move";

        public const string STARTUCI = "uci";
        public const string MANSTARTUCI = "Starts UCI server";

        public const string ENTERFEN = "fen";
        public const string MANENTERFEN = "Allows the user to enter a chess position using FEN";

        public const string GETFEN = "get";
        public const string MANGETFEN = "Gives the FEN string from the current chess position";

        public const string EXIT = "exit";
        public const string MANEXIT = "Closes the program";

        public static string[,] listCommands()
        {
            //string[] commands = {HELP, SHOWBOARD, STARTUCI, ENTERFEN, GETFEN, EXIT};
            string[,] commands = new string[7, 2] 
            {
                {HELP, MANHELP}, {SHOWBOARD, MANSHOWBOARD}, {STARTAI, MANSTARTAI}, {STARTUCI, MANSTARTUCI},
                {ENTERFEN, MANENTERFEN}, {GETFEN, MANGETFEN}, {EXIT, MANEXIT}
            };

            for(int i = 0; i < commands.GetLength(0); i++)
            {
                Console.Write(">");
                for(int j = 0; j < commands.GetLength(1); j++)
                {
                    System.Console.WriteLine(commands[i,j]);
                }
            }
            return commands;
        }
    }
}