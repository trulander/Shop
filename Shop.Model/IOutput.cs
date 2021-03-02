using System;
using System.Threading;

namespace Shop.Model
{
    public interface IOutput
    {
        public string lastMethodRequired{ get; set; }
        public EventWaitHandle[] waitHandle { get; set; }
        public int ConsoleKey { get; set; }
        public string ConsoleText { get; set; }
        public string Buffer { get; }
        public void Write(string text = "", ConsoleColor color = ConsoleColor.Gray);
        public void WriteLine(string text = "", ConsoleColor color = ConsoleColor.Gray);
        public void Clear();
        public string ReadLine();
        public int ReadKey();
    }
}