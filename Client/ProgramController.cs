using System;

namespace Client
{
    public class ProgramController
    {
        public ProgramController()
        {
            Console.WriteLine("Hello World!");

            HttpController httpController = new HttpController();
        }
    }
}