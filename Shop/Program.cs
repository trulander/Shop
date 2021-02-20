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
            ShopController shop = new ShopController();
            shop.Login();

            Console.OutputEncoding = Encoding.UTF8;

            do
            {
                Console.Clear();
                Output.WriteLine(shop.CurrentMenu.GetFullPathText(), ConsoleColor.Yellow);//U+00BB
                Console.WriteLine();

                for (int i = 0; i < shop.CurrentMenu.Children.Count; i++)
                {
                    IMenuItem item = shop.CurrentMenu.Children[i];

                    if (i != shop.SelectedMenuIndex)
                        Console.WriteLine("  " + item.Text);
                    else
                        Output.WriteLine("\u00A7 " + item.Text, ConsoleColor.Cyan);
                }

                switch(Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        switch (shop.CurrentMenu.Children[shop.SelectedMenuIndex])
                        {
                            case ActionMenuItem action:
                                shop.RouteTo(action.Command);
                                break;
                            case ContainerMenuItem container:
                                shop.CurrentMenu = container;
                                shop.SelectedMenuIndex = 0;
                                break;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (shop.SelectedMenuIndex > 0)
                            shop.SelectedMenuIndex--;
                        else
                            shop.SelectedMenuIndex = shop.CurrentMenu.Children.Count - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if (shop.SelectedMenuIndex < shop.CurrentMenu.Children.Count - 1)
                            shop.SelectedMenuIndex++;
                        else
                            shop.SelectedMenuIndex = 0;
                        break;

                    case ConsoleKey.Backspace:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.Escape:
                        if (shop.SelectedMenuIndex > 0)
                            shop.SelectedMenuIndex = 0;
                        else
                            if (shop.CurrentMenu.Parent != null)
                                shop.CurrentMenu = shop.CurrentMenu.Parent;
                        break;
                    case ConsoleKey.RightArrow:
                        if (shop.CurrentMenu.Children[shop.SelectedMenuIndex] is ContainerMenuItem containerItem)
                        {
                            shop.CurrentMenu = containerItem;
                            shop.SelectedMenuIndex = 0;
                        }                            
                        break;
                }
            }
            while (shop.IsLoggedIn);
        }
    }
}
