using System;

namespace Client
{
    public class View
    {
        public static void Write(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            Write(text, color);
            Console.WriteLine();
        }
    }
}