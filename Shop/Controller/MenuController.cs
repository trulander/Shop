using Shop.Model;
using System.Collections.Generic;

namespace Shop.Controller
{
    class MenuController
    {
        public IContainerMenuItem Current { get; private set; }
        public int SelectedIndex { get; private set; } = 0;
        private IOutput _output;

        public MenuController(IOutput output)
        {
            _output = output;
            Current = new ContainerMenuItem("Магазин", new List<IMenuItem>()
            {
                new ContainerMenuItem("Витрины/Полки", new List<IMenuItem>(){
                    new ActionMenuItem("Показать активные", "showcase.show"),
                    new ActionMenuItem("Показать корзину", "showcase.trash"),
                    new ActionMenuItem("Добавить", "showcase.create"),
                    new ActionMenuItem("Изменить", "showcase.edit"),
                    new ActionMenuItem("Удалить", "showcase.remove"),
                    new ContainerMenuItem("Дополнительно", new List<IMenuItem>(){
                        new ActionMenuItem("Показать товары на витрине", "showcase.products"),
                        new ActionMenuItem("Разместить товар на витрине", "showcase.place_product"),
                    }),
                }),
                new ContainerMenuItem("Товары", new List<IMenuItem>(){
                    new ActionMenuItem("Показать все", "product.show"),
                    new ActionMenuItem("Добавить", "product.create"),
                    new ActionMenuItem("Изменить", "product.edit"),
                    new ActionMenuItem("Удалить", "product.remove")
                }),
                new ActionMenuItem("Выход", "app.exit")
            });
        }

        public void Prev()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
            else
                SelectedIndex = (Current.Children.Count > 0) ? Current.Children.Count - 1 : 0;
        }
        public void Next()
        {
            if (SelectedIndex < Current.Children.Count - 1)
                SelectedIndex++;
            else
                SelectedIndex = 0;
        }
        public void Return()
        {
            if (SelectedIndex > 0)
                SelectedIndex = 0;
            else
                if (Current.Parent != null)
                    Current = Current.Parent;
        }

        public void Expand(IContainerMenuItem container)
        {
            Current = container;
            SelectedIndex = 0;
        }
    }
}
