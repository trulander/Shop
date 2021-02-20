using Shop.Model;
using System;
using System.Collections.Generic;

namespace Shop.Controller
{
    class ShopController
    {
        public bool IsLoggedIn { get; private set; }

        public List<Product> Products { get; set; }
        int _lastInsertedProductId = 0;

        public List<Showcase> Showcases { get; set; }
        int _lastInsertedShowcaseId = 0;

        public ContainerMenuItem Menu { get; set; }
        public ContainerMenuItem CurrentMenu;
        public int SelectedMenuIndex { get; set; } = 0;

        public void Login() => IsLoggedIn = true;
        public void Logout() => IsLoggedIn = false;

        public ShopController()
        {
            Menu = new ContainerMenuItem("Магазин", new List<IMenuItem>()
            {
                new ContainerMenuItem("Витрины", new List<IMenuItem>(){
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

            CurrentMenu = Menu;

            Products = new List<Product>();
            Showcases = new List<Showcase>();

            Seed();
        }

        /// <summary>
        /// Заполняет списки начальными данными
        /// </summary>
        /// <param name="products">Заполнять товары</param>
        /// <param name="showcases">Заполнять витрины</param>
        void Seed(bool products = true, bool showcases = true)
        {
            if (products)
                for (int i = 0; i < 4; i++)
                {
                    Products.Add(new Product()
                    {
                        Id = ++_lastInsertedProductId,
                        Name = "Товар " + (i + 1),
                        Capacity = 1 + i
                    });
                }

            if (showcases)
                for (int i = 0; i < 4; i++)
                {
                    Showcases.Add(new Showcase()
                    {
                        Id = ++_lastInsertedShowcaseId,
                        Name = "Витрина " + (i + 1),
                        MaxCapacity = 1 + i,
                        CreatedAt = DateTime.Now
                    });
                }
        }

        public void RouteTo(string command)
        {
            string[] data = command.Split('.');
            string controller = data[0];
            string action = data[1];

            switch (controller)
            {
                case "product":
                    switch(action)
                    {
                        case "show":
                            PrintProductsAction();
                            break;

                        case "create":
                            ShowResult(ProductCreateAction());
                            break;

                        case "edit":
                            ShowResult(ProductUpdateAction());
                            break;

                        case "remove":
                            ShowResult(ProductRemoveAction());
                            break;
                    }
                    break;

                case "showcase":
                    switch (action)
                    {
                        case "show":
                            PrintShowcasesAction();
                            break;

                        case "create":
                            ShowResult(ShowcaseCreateAction());
                            break;

                        case "edit":
                            ShowResult(ShowcaseUpdateAction());
                            break;

                        case "remove":
                            ShowResult(ShowcaseRemoveAction());
                            break;

                        case "place_product":
                            ShowResult(PlaceProductAction());
                            break;

                        case "products":
                            PrintShowcaseProductsAction();
                            break;
                        case "trash":
                            PrintShowcasesAction(showOnlyDeleted: true);
                            break;
                    }
                    break;
                case "app":
                    switch (action)
                    {
                        case "exit":
                            Logout();
                            break;
                    }
                    break;
            }
        }

        #region Actions

        /// <summary>
        /// Вызывает сценарий создания товара
        /// </summary>
        /// <returns></returns>
        IActionResult ProductCreateAction()
        {
            IActionResult result = new ActionResult(false, string.Empty);
            Output.WriteLine("\r\nДобавить товар:", ConsoleColor.Yellow);
            Product p = new Product();
            Output.Write("Наименование:");
            p.Name = Console.ReadLine();
            Output.Write("Занимаемый объем:");

            if (int.TryParse(Console.ReadLine(), out int capacity))
                p.Capacity = capacity;

            IValidateResult validateResult = p.Validate();

            if (validateResult.Success)
            {
                p.Id = ++_lastInsertedProductId;
                Products.Add(p);
                result.Success = true;
            }
            else result.Message = validateResult.Message;

            return result;
        }

        /// <summary>
        /// Вызывает сценарий изменения товара
        /// </summary>
        /// <returns></returns>
        IActionResult ProductUpdateAction()
        {
            PrintProductsAction(false);

            IActionResult result = new ActionResult(false, "");

            Output.Write("\r\nВведите id товара: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int pid))
            {
                int pIdx = IndexOfProduct(pid);

                if (pIdx >= 0)
                {
                    Product product = new Product();

                    Output.Write("Наименование (" + Products[pIdx].Name + "):");
                    string name = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(name))
                        product.Name = name;
                    else
                        product.Name = Products[pIdx].Name;

                    //Не даем возможность менять объем товара размещенного на витрине
                    if (!ProductPlacedInShowcase(product.Id))
                    {
                        Output.Write("Занимаемый объем (" + Products[pIdx].Capacity + "):");

                        string capacity = Console.ReadLine();
                        if (int.TryParse(capacity, out int capacityInt))
                            product.Capacity = capacityInt;
                        else
                            product.Capacity = Products[pIdx].Capacity;
                    }

                    IValidateResult validateResult = product.Validate();

                    if (validateResult.Success)
                    {
                        product.Id = pid;
                        Products[pIdx] = product;
                        result.Success = true;
                    }
                    else result.Message = validateResult.Message;
                }
                else result.Message = "Товар с идентификатором " + pid + " не найден";
            }
            else result.Message = "Идентификатор должен быть целым положительным числом";

            return result;
        }

        /// <summary>
        /// Вызывает сценарий удаления товара
        /// </summary>
        /// <returns></returns>
        IActionResult ProductRemoveAction()
        {
            IActionResult result = new ActionResult(false, "");

            PrintProductsAction(false);

            Output.Write("\r\nВведите Id товара: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
            {
                int idx = IndexOfProduct(id);
                if (idx >= 0)
                {
                    for (int i = 0; i < Showcases.Count; i++)
                    {
                        if (Showcases[i].HasProduct(id))
                            Showcases[i].ProductRemove(id);
                    }
                    Products.RemoveAt(idx);
                    result.Success = true;
                }
                else result.Message = "Товар с идентификатором " + id + " не найден";
            }
            else result.Message = "Идентификатор должен быть целым положительным числом";

            return result;
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        IActionResult ShowcaseCreateAction()
        {
            IActionResult result = new ActionResult(false, "");
            Console.Clear();
            Output.WriteLine("Добавить витрину", ConsoleColor.Yellow);
            Showcase showcase = new Showcase();
            Output.Write("Наименование: ");
            showcase.Name = Console.ReadLine();
            Output.Write("Максимально допустимый объем витрины: ");

            if (int.TryParse(Console.ReadLine(), out int maxCapacity))
                showcase.MaxCapacity = maxCapacity;

            IValidateResult validateResult = showcase.Validate();

            if (validateResult.Success)
            {
                showcase.CreatedAt = DateTime.Now;
                showcase.Id = ++_lastInsertedShowcaseId;
                Showcases.Add(showcase);
                result.Success = true;
            }
            else result.Message = validateResult.Message;

            return result;
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        IActionResult ShowcaseUpdateAction()
        {
            IActionResult result = new ActionResult(false, string.Empty);

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                int idx = IndexOfActiveShowcase(id);

                if (idx >= 0)
                {
                    Showcase showcase = new Showcase();

                    Output.Write("Наименование (" + Showcases[idx].Name + "):");
                    showcase.Name = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(showcase.Name))
                        showcase.Name = Showcases[idx].Name;

                    Output.Write("Максимально допустимый объем витрины (" + Showcases[idx].MaxCapacity + "):");

                    //Если объем задан корректно, то применяем, в противном случае оставляем как было
                    if (int.TryParse(Console.ReadLine(), out int capacityInt))
                        showcase.MaxCapacity = capacityInt;
                    else 
                        showcase.MaxCapacity = Showcases[idx].MaxCapacity;

                    //Чекаем изменение объема в меньшую сторону
                    if (Showcases[idx].GetProductsCapacity(Products) <= showcase.MaxCapacity)
                    {
                        IValidateResult validateResult = showcase.Validate();

                        if (validateResult.Success)
                        {
                            showcase.Id = id;
                            Showcases[idx] = showcase;
                            result.Success = true;
                        }
                        else result.Message = validateResult.Message;
                    }
                    else result.Message = "Невозможно установить заданный объем, объем размещенного товара превышеает значение";
                }
                else result.Message = "Витрина с идентификатором " + id + " не найдена";
            }
            else result.Message = "Идентификатор должен быть целым положительным числом";

            return result;
        }

        /// <summary>
        /// Вызывает сценарий удаления витрины
        /// </summary>
        /// <returns></returns>
        IActionResult ShowcaseRemoveAction()
        {
            Console.Clear();
            Output.WriteLine("Удалить витрину", ConsoleColor.Cyan);

            if (ShowcasesCount() == 0)
                return new ActionResult(false, "Нет витрин для удаления");

            PrintShowcasesAction(false);

            IActionResult result = new ActionResult(false,"");

            Output.Write("\r\nВведите Id витрины для удаления: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
            {
                for (int i = 0; i < Showcases.Count; i++)
                    if (Showcases[i].Id.Equals(id))
                    {
                        if (Showcases[i].GetProductsIds().Count == 0)
                        {
                            Showcases[i].RemovedAt = DateTime.Now;
                            result.Success = true;
                        }
                        else 
                            result.Message = "Невозможно удалить витрину, на которой размещены товары";

                        break;
                    }
            }
            else result.Message = "Идентификатор должен быть ценым положительным числом";
                
            return result;
        }

        /// <summary>
        /// Вызывает сценарий размещения товара на витрине
        /// </summary>
        /// <returns></returns>
        IActionResult PlaceProductAction()
        {
            Console.Clear();

            if (ShowcasesCount() == 0 || Products.Count == 0)
                return new ActionResult(false, "Нет товара и витрин для отображения");

            IActionResult result = new ActionResult(false, "");

            Output.Write("Размещение товара на витрине", ConsoleColor.Yellow);

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ");
            if (int.TryParse(Console.ReadLine(), out int scId) && scId > 0)
            {
                int scIdx = IndexOfActiveShowcase(scId);

                if (scIdx >= 0)
                {
                    Console.Clear();
                    PrintProductsAction(false);

                    Output.Write("\r\nВведите Id товара: ");
                    if (int.TryParse(Console.ReadLine(), out int pId) && pId > 0)
                    {
                        int pIdx = IndexOfProduct(pId);

                        if (pIdx >= 0)
                        {
                            Output.Write("Выбран товар ");
                            Output.WriteLine(Products[pIdx].Name, ConsoleColor.Cyan);

                            Output.Write("Введите количество: ");
                            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                            {
                                Output.Write("Введите стоимость: ");
                                if (int.TryParse(Console.ReadLine(), out int cost) && cost > 0)
                                {
                                    IValidateResult validateResult = Showcases[scIdx].ProductPlace(Products[pIdx], quantity, cost);
                                    if (validateResult.Success)
                                        result.Success = true;
                                    else
                                        result.Message = validateResult.Message;
                                }
                                else result.Message = "Стоимость товара должна быть положительным числом";
                            }
                            else result.Message = "Количество товара должно быть положительным числом";
                        }
                        else result.Message = "Товара с идентификатором " + pId + " не найдено";
                    }
                    else result.Message = "Идентификатор товара должен быть положительным числом";
                }
                else result.Message = "Витрины с идентификатором " + scId + " не найдено";
            } 
            else result.Message = "Идентификатор витрины должен быть положительным числом";

            return result;
        }

        /// <summary>
        /// Вызывает сценарий отображения всех товаров
        /// </summary>
        /// <returns></returns>
        void PrintProductsAction(bool waitPressKey = true)
        {
            Console.Clear();

            if (Products.Count == 0)
            {
                Output.WriteLine("Нет товаров для отображения");
                Console.ReadKey();
                return;
            }

            Output.WriteLine("Доступные товары", ConsoleColor.Cyan);            
            foreach (Product product in Products)
                Output.WriteLine(product.Print());

            if (waitPressKey)
                Console.ReadKey();
        }

        /// <summary>
        /// Вызывает сценарий отображения всех товаров на витрине
        /// </summary>
        /// <returns></returns>
        void PrintShowcaseProductsAction(bool waitPressKey = true)
        {
            if (ShowcasesCount() == 0 || Products.Count == 0)
            {
                Console.Clear();
                Output.WriteLine("Нет товаров и витрин для отображения");
                Console.ReadKey();
                return;
            }

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);

            if (int.TryParse(Console.ReadLine(), out int scId))
            {
                int scIdx = IndexOfActiveShowcase(scId);

                if (scIdx >= 0)
                {
                    Output.Write("\r\nТовары на витрине ");
                    Output.WriteLine(Showcases[scIdx].Name + ":", ConsoleColor.Cyan);

                    List<int> ids = Showcases[scIdx].GetProductsIds();

                    if (ids.Count > 0)
                    {
                        foreach (int pId in ids)
                        {
                            int pIdx = IndexOfProduct(pId);
                            if (pIdx >= 0)
                                Output.WriteLine(Products[pIdx].Print());
                        }
                    }
                    else Output.WriteLine("Нет товаров для отображения");
                } 
                else Output.WriteLine("Нет витрин с указанным идентификатором");
            }
            else Output.WriteLine("Нет витрин с указанным идентификатором");

            if (waitPressKey)
                Console.ReadKey();
        }

        /// <summary>
        /// Вызывает сценарий отображения витрин
        /// </summary>
        /// <returns></returns>
        void PrintShowcasesAction(bool waitPressKey = true, bool showOnlyDeleted = false)
        {
            Console.Clear();

            if (showOnlyDeleted)
                Output.WriteLine("Витрины в корзине", ConsoleColor.Yellow);
            else
                Output.WriteLine("Доступные витрины", ConsoleColor.Yellow);

            if (ShowcasesCount(showOnlyDeleted) == 0)
            {
                Output.WriteLine("Нет витрин для отображения");
                Console.ReadKey();
                return;
            }

            foreach (Showcase showcase in Showcases)
            {
                if ((showOnlyDeleted && showcase.RemovedAt != null) || (!showOnlyDeleted && showcase.RemovedAt == null))
                    Output.WriteLine(showcase.Print());
            }                

            if (waitPressKey)
                Console.ReadKey();
        }

        #endregion

        /// <summary>
        /// Возвращает индекс товара по id в списке товаров
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int IndexOfProduct(int id)
        {
            int result = -1;
            for (int i = 0; i < Products.Count; i++)
                if (Products[i].Id.Equals(id))
                {
                    result = i;
                    break;
                }
            return result;
        }

        /// <summary>
        /// Возвращает индекс витрины по id в списке витрин
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="findInActiveShowcases">Искать только среди активных витрин</param>
        /// <returns></returns>
        int IndexOfActiveShowcase(int id)
        {
            int result = -1;
            for (int i = 0; i < Showcases.Count; i++)
                //Ищем только среди активных витрин
                if (Showcases[i].Id.Equals(id) && Showcases[i].RemovedAt == null)
                {
                    result = i;
                    break;
                }
            return result;
        }

        /// <summary>
        /// Возвращает количество витрин
        /// </summary>
        /// <param name="deleted">Установите в True если необходимо вернуть количество удаленных витри, False - активных</param>
        /// <returns></returns>
        int ShowcasesCount(bool deleted = false)
        {
            int count = 0;

            foreach (Showcase item in Showcases)
            {
                if (!deleted == (item.RemovedAt == null))
                    count++;
            }

            return count;
        }

        bool ProductPlacedInShowcase(int id)
        {
            for (int i = 0; i < Showcases.Count; i++)
                if (Showcases[i].RemovedAt == null && Showcases[i].HasProduct(id))
                    return true;

            return false;
        }

        /// <summary>
        /// Выводит на экран сообщение о результате выполнения действия
        /// </summary>
        /// <param name="result"></param>
        void ShowResult(IActionResult result)
        {
            if (result.Success && string.IsNullOrWhiteSpace(result.Message))
                result.Message = "Выполнено";

            Output.WriteLine(result.Message, result.Success ? ConsoleColor.Green : ConsoleColor.Red);

            Output.WriteLine("Нажмите любую клавишу для продолжения..", ConsoleColor.Yellow);
            Console.ReadKey();
        }
    }
}
