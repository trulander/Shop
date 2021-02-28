using Shop.Controller;
using Shop.Model;
using System;
using System.Text;

namespace Shop
{
    class Program
    {
        static void Main(string[] args)
        {
            var menu = new MenuController();
            var shop = new ShopController();
            shop.Login();

            Console.OutputEncoding = Encoding.UTF8;

            do
            {
                Console.Clear();
                Output.WriteLine(menu.Current.GetFullPathText(), ConsoleColor.Yellow);
                Console.WriteLine();

                for (int i = 0; i < menu.Current.Children.Count; i++)
                {
                    IMenuItem item = menu.Current.Children[i];

                    if (i != menu.SelectedIndex)
                        Console.WriteLine("  " + item.Text);
                    else
                        Output.WriteLine("\u00A7 " + item.Text, ConsoleColor.Cyan);
                }

                switch(Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        switch (menu.Current.Children[menu.SelectedIndex])
                        {
                            case ActionMenuItem action:
                                shop.RouteTo(action.Command);
                                break;
                            case IContainerMenuItem container:
                                menu.Expand(container);
                                break;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        menu.Prev();
                        break;
                    case ConsoleKey.DownArrow:
                        menu.Next();
                        break;

                    case ConsoleKey.Backspace:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.Escape:
                        menu.Return();
                        break;
                    case ConsoleKey.RightArrow:
                        if (menu.Current.Children[menu.SelectedIndex] is IContainerMenuItem containerItem)
                            menu.Expand(containerItem);
                        break;
                }
            }
            while (shop.IsLoggedIn);
        }
    }
}
