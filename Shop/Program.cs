using System;
using System.Text;
using Shop.Controller;

namespace Shop
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerController server = new ServerController(new OutputServer());
            ProgramController program = new ProgramController(new Output());
            program.StartProgram();
        }
    }
}
