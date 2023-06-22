using System;
using Util;

namespace Uci
{
    public class UciInterface
    {
        public static void startUci()
        {
            System.Console.WriteLine("Starting UCI Server");
            //TODO: add uci support

            while(true)
            {
                //looks to see if gui said something
                string? inputString = Console.ReadLine();

                switch (inputString)
                {
                    case(UciCommands.READYOK):
                        //do something
                        break;
                    case "":
                        //do something
                        break;
                    default:
                        // if it doesn't recognice the command it does nothing
                        break;
                }
            }
        }
    }
}