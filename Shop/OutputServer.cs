using System;
using System.Threading;
using Shop.Model;

namespace Shop
{
    public class OutputServer : IOutput
    {
        public string lastMethodRequired { get; set; }
        public EventWaitHandle[] waitHandle { get; set; }
        public int ConsoleKey { get; set; }
        public string ConsoleText { get; set; }
        public string Buffer { get; private set; }
        public void Write(string text = "", ConsoleColor color = ConsoleColor.Gray)
        {
            Buffer += text + "\r\n";;
        }

        public void WriteLine(string text = "", ConsoleColor color = ConsoleColor.Gray)
        {
            Buffer += text + "\r\n";
        }

        public void Clear()
        {
            Buffer = "";
            Console.Clear();
        }

        public string ReadLine()
        {
            lastMethodRequired = "ReadLine";
            if (ConsoleText == "")
            {
                waitHandle[0].Reset();
                waitHandle[1].Set();
                waitHandle[0].WaitOne();
                lastMethodRequired = "ReadKey";
            }
            var result = ConsoleText;
            ConsoleText = "";
            return result;
        }

        public int ReadKey()
        {
            lastMethodRequired = "ReadKey";
            if (ConsoleKey == 0)
            {
                waitHandle[0].Reset();
                waitHandle[1].Set();
                waitHandle[0].WaitOne();
                lastMethodRequired = "ReadKey";
            }
            var result = ConsoleKey;
            ConsoleKey = 0;
            return result;
        }
    }
}