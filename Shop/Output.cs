using Shop.Model;
using System;

namespace Shop
{
    class Output
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

        /// <summary>
        /// Выводит на экран сообщение о результате выполнения действия
        /// </summary>
        /// <param name="result"></param>
        public static void ShowResult(IResult result)
        {
            if (result.Success && string.IsNullOrWhiteSpace(result.Message))
                result.Message = "Выполнено";

            WriteLine(result.Message, result.Success ? ConsoleColor.Green : ConsoleColor.Red);

            WriteLine("Нажмите любую клавишу для продолжения..", ConsoleColor.Yellow);
            Console.ReadKey();
        }
    }
}
