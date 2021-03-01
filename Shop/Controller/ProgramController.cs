using Shop.Model;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shop.Model;

namespace Shop.Controller
{
    public class ProgramController
    {
        private MenuController _menu;
        private ShopController _shop;
        private ServerController _server;
        
        public ProgramController()
        {
            _menu = new MenuController();
            _shop = new ShopController();
            _shop.Login();
            
            var shopIsLoggedIn = _shop.IsLoggedIn;
            _server = new ServerController(_shop,_menu);

            Console.OutputEncoding = Encoding.UTF8;
            
            MainLoop();
        }

        public void MainLoop()
        {
            do
            {
                Console.Clear();
                Output.WriteLine(_menu.Current.GetFullPathText(), ConsoleColor.Yellow);
                Console.WriteLine();

                for (int i = 0; i < _menu.Current.Children.Count; i++)
                {
                    IMenuItem item = _menu.Current.Children[i];

                    if (i != _menu.SelectedIndex)
                        Console.WriteLine("  " + item.Text);
                    else
                        Output.WriteLine("\u00A7 " + item.Text, ConsoleColor.Cyan);
                }

                switch(Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        switch (_menu.Current.Children[_menu.SelectedIndex])
                        {
                            case ActionMenuItem action:
                                _shop.RouteTo(action.Command);
                                break;
                            case IContainerMenuItem container:
                                _menu.Expand(container);
                                break;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        _menu.Prev();
                        break;
                    case ConsoleKey.DownArrow:
                        _menu.Next();
                        break;

                    case ConsoleKey.Backspace:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.Escape:
                        _menu.Return();
                        break;
                    case ConsoleKey.RightArrow:
                        if (_menu.Current.Children[_menu.SelectedIndex] is IContainerMenuItem containerItem)
                            _menu.Expand(containerItem);
                        break;
                }
            }
            while (_shop.IsLoggedIn);
        }
    }
}