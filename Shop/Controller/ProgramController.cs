using Shop.Model;
using System;

namespace Shop.Controller
{
    public class ProgramController
    {
        private MenuController _menu;
        private ShopController _shop;
        
        private IOutput _output;
        
        public ProgramController(IOutput output)
        {
            _output = output;
            _menu = new MenuController(_output);
            _shop = new ShopController(_output);
        }
        public void StartProgram()
        {
            _shop.Login();
            RefrashView();
            MainLoop();
        }

        public void MainLoop()
        {
            do
            {
                
                switch(_output.ReadKey())
                {
                    case (int)ConsoleKey.Enter:
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
                    case (int)ConsoleKey.UpArrow:
                        _menu.Prev();
                        break;
                    case (int)ConsoleKey.DownArrow:
                        _menu.Next();
                        break;

                    case (int)ConsoleKey.Backspace:
                    case (int)ConsoleKey.LeftArrow:
                    case (int)ConsoleKey.Escape:
                        _menu.Return();
                        break;
                    case (int)ConsoleKey.RightArrow:
                        if (_menu.Current.Children[_menu.SelectedIndex] is IContainerMenuItem containerItem)
                            _menu.Expand(containerItem);
                        break;
                }

                RefrashView();
            }
            while (_shop.IsLoggedIn);
        }

        private void RefrashView()
        {
            _output.Clear();
            _output.WriteLine(_menu.Current.GetFullPathText(), ConsoleColor.Yellow);
            _output.WriteLine();

            for (int i = 0; i < _menu.Current.Children.Count; i++)
            {
                IMenuItem item = _menu.Current.Children[i];

                if (i != _menu.SelectedIndex)
                    _output.WriteLine("  " + item.Text);
                else
                    _output.WriteLine("\u00A7 " + item.Text, ConsoleColor.Cyan);
            }
        }
    }
}