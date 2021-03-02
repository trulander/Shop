using System;
using System.Text;
using System.Threading;
using Shop.Model;

namespace Shop
{
    class Output : IOutput
    {
        public Output()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        public string lastMethodRequired { get; set; }
        public EventWaitHandle[] waitHandle { get; set; }
        public int ConsoleKey { get; set; }
        public string ConsoleText { get; set; }
        public string Buffer { get; private set; }

        public void Write(string text = "", ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public void WriteLine(string text = "", ConsoleColor color = ConsoleColor.Gray)
        {
            Write(text, color);
            Console.WriteLine();
        }

        public void Clear()
        {
            Console.Clear();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public int ReadKey()
        {
            return (int)Console.ReadKey(true).Key;
        }
    }
}
