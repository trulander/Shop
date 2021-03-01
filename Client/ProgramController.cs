using System;

namespace Client
{
    public class ProgramController
    {
        public ProgramController()
        {
            Console.WriteLine("Http client for ShowCase developed by code1code.");
            MainLoop();
        }

        private void MainLoop()
        {
            
            do
            {
                
                //View.WriteLine(_menu.Current.GetFullPathText(), ConsoleColor.Yellow);
                //Console.WriteLine();

                /*for (int i = 0; i < _menu.Current.Children.Count; i++)
                {
                    IMenuItem item = _menu.Current.Children[i];

                    if (i != _menu.SelectedIndex)
                        Console.WriteLine("  " + item.Text);
                    else
                        Output.WriteLine("\u00A7 " + item.Text, ConsoleColor.Cyan);
                }*/
                int action = (int)Console.ReadKey(true).Key;
                Console.Clear();
                HttpController httpController = new HttpController(action);
               
            }
            while (true);
        }
    }
}