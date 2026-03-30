using System;
using System.Threading;

namespace CyberAwarenessBot.UI
{
    public static class ConsoleHelper
    {
        public static void PrintColour(string message, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void PrintDivider()
        {
            PrintColour("══════════════════════════════════════════════", ConsoleColor.DarkCyan);
        }

        public static void TypeWrite(string message, ConsoleColor colour = ConsoleColor.White)
        {
            Console.ForegroundColor = colour;
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
