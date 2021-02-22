using Shop.DAL;
using Shop.Model;
using System;
using System.Collections.Generic;

namespace Shop.Controller
{
    class ShopController
    {
        public bool IsLoggedIn { get; private set; }

        private IProductRepository ProductRepository { get; set; }
        private IShowcaseRepository ShowcaseRepository { get; set; }

        //public List<Showcase> Showcases { get; set; }
        //int _lastInsertedShowcaseId = 0;

        public void Login() => IsLoggedIn = true;
        public void Logout() => IsLoggedIn = false;

        public ShopController()
        {
            ProductRepository = new ProductRepository();
            ProductRepository.Seed(2);

            ShowcaseRepository = new ShowcaseRepository();
            ShowcaseRepository.Seed(2);
        }

        //Будут делегаты, будет жизнь
        public void RouteTo(string command)
        {
            switch (command)
            {
                case "product.show":
                    PrintProductsAction();
                    break;
                case "product.create":
                    ShowResult(ProductCreateAction());
                    break;
                case "product.edit":
                    ShowResult(ProductUpdateAction());
                    break;
                case "product.remove":
                    ShowResult(ProductRemoveAction());
                    break;

                case "showcase.show":
                    PrintShowcasesAction();
                    break;
                case "showcase.create":
                    ShowResult(ShowcaseCreateAction());
                    break;
                case "showcase.edit":
                    ShowResult(ShowcaseUpdateAction());
                    break;
                case "showcase.remove":
                    ShowResult(ShowcaseRemoveAction());
                    break;
                case "showcase.place_product":
                    ShowResult(PlaceProductAction());
                    break;
                case "showcase.products":
                    PrintShowcaseProductsAction();
                    break;
                case "showcase.trash":
                    PrintShowcasesAction(showOnlyDeleted: true);
                    break;

                case "app.exit":
                     Logout();
                    break;
            }
        }

        #region Actions

        /// <summary>
        /// Вызывает сценарий создания товара
        /// </summary>
        /// <returns></returns>
        IResult ProductCreateAction()
        {
            IResult result = new Result();
            Output.WriteLine("\r\nДобавить товар:", ConsoleColor.Yellow);
            Product p = new Product();
            Output.Write("Наименование:");
            p.Name = Console.ReadLine();
            Output.Write("Занимаемый объем:");

            if (int.TryParse(Console.ReadLine(), out int capacity))
                p.Capacity = capacity;

            IResult validateResult = p.Validate();

            if (!validateResult.Success)
                return validateResult;

            ProductRepository.Add(p);
            result.Success = true;

            return result;
        }

        /// <summary>
        /// Вызывает сценарий изменения товара
        /// </summary>
        /// <returns></returns>
        IResult ProductUpdateAction()
        {
            PrintProductsAction(false);

            IResult result = new Result();

            Output.Write("\r\nВведите id товара: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int pid))
            {
                Product product = ProductRepository.GetById(pid);

                if (product == null)
                    return new Result() { Message = "Товар с идентификатором " + pid + " не найден" };

                Output.Write("Наименование (" + product.Name + "):");
                string name = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                    product.Name = name;

                //Не даем возможность менять объем товара размещенного на витрине

                bool placedInShowcase = false;

                foreach (Showcase showcase in ShowcaseRepository.All())
                    if (ShowcaseRepository.GetShowcaseProductsIds(showcase).Count > 0)
                    {
                        placedInShowcase = true;
                        break;
                    }

                if (!placedInShowcase)
                {
                    Output.Write("Занимаемый объем (" + product.Capacity + "):");

                    if (int.TryParse(Console.ReadLine(), out int capacityInt))
                        product.Capacity = capacityInt;
                }

                IResult validateResult = product.Validate();

                if (!validateResult.Success)
                    return validateResult;

                ProductRepository.Update(product);
                result.Success = true;
            }
            else result.Message = "Идентификатор должен быть целым положительным числом";

            return result;
        }

        /// <summary>
        /// Вызывает сценарий удаления товара
        /// </summary>
        /// <returns></returns>
        IResult ProductRemoveAction()
        {
            IResult result = new Result();

            PrintProductsAction(false);

            Output.Write("\r\nВведите Id товара: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
            {
                Product product = ProductRepository.GetById(id);

                if (product == null)
                    return new Result() { Message = "Товар с идентификатором " + id + " не найден" };

                ShowcaseRepository.TakeOut(product);
                ProductRepository.Remove(id);
                result.Success = true;
            }
            else result.Message = "Идентификатор должен быть целым положительным числом";

            return result;
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        IResult ShowcaseCreateAction()
        {
            IResult result = new Result();
            Console.Clear();
            Output.WriteLine("Добавить витрину", ConsoleColor.Yellow);
            Showcase showcase = new Showcase();
            Output.Write("Наименование: ");
            showcase.Name = Console.ReadLine();
            Output.Write("Максимально допустимый объем витрины: ");

            if (int.TryParse(Console.ReadLine(), out int maxCapacity))
                showcase.MaxCapacity = maxCapacity;

            IResult validateResult = showcase.Validate();

            if (!validateResult.Success)
                return validateResult;
             
            ShowcaseRepository.Add(showcase);
            result.Success = true;

            return result;
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        IResult ShowcaseUpdateAction()
        {
            IResult result = new Result();

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Showcase showcase = ShowcaseRepository.GetById(id);

                if (showcase == null || showcase.RemovedAt.HasValue)
                    return new Result() { Message = "Витрина с идентификатором " + id + " не найдена" };

                Output.Write("Наименование (" + showcase.Name + "):");
                string name = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                    showcase.Name = name;

                Output.Write("Максимально допустимый объем витрины (" + showcase.MaxCapacity + "):");

                //Если объем задан корректно, то применяем, в противном случае оставляем как было
                if (int.TryParse(Console.ReadLine(), out int capacityInt))
                {
                    //Чекаем изменение объема в меньшую сторону
                    List<ProductShowcase> productShowcases = ShowcaseRepository.GetShowcaseProducts(showcase);
                    int showcaseFullness = ProductRepository.ProductsCapacity(productShowcases);

                    if (showcaseFullness > capacityInt)
                        return new Result() { Message = "Невозможно установить заданный объем, объем размещенного товара превышеает значение" };

                    showcase.MaxCapacity = capacityInt;

                    IResult validateResult = showcase.Validate();

                    if (!validateResult.Success)
                        return validateResult;
                
                    ShowcaseRepository.Update(showcase);
                    result.Success = true;
                } 
                else result.Message = "Объем должен быть целым положительным числом";    
            }
            else result.Message = "Идентификатор должен быть целым положительным числом";

            return result;
        }

        /// <summary>
        /// Вызывает сценарий удаления витрины
        /// </summary>
        /// <returns></returns>
        IResult ShowcaseRemoveAction()
        {
            Console.Clear();
            Output.WriteLine("Удалить витрину", ConsoleColor.Cyan);

            if (ShowcaseRepository.ActivesCount() == 0)
                return new Result() { Message = "Нет витрин для удаления" };

            PrintShowcasesAction(false);

            IResult result = new Result();

            Output.Write("\r\nВведите Id витрины для удаления: ", ConsoleColor.Yellow);
            if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
            {
                Showcase showcase = ShowcaseRepository.GetById(id);

                if (showcase == null)
                    return new Result() { Message = "Витрина не найдена" };
             
                if (ShowcaseRepository.GetShowcaseProductsIds(showcase).Count != 0)
                    return new Result() { Message = "Невозможно удалить витрину, на которой размещены товары" };
                     
                ShowcaseRepository.Remove(showcase.Id);
                result.Success = true;
            }
            else result.Message = "Идентификатор должен быть ценым положительным числом";
                
            return result;
        }

        /// <summary>
        /// Вызывает сценарий размещения товара на витрине
        /// </summary>
        /// <returns></returns>
        IResult PlaceProductAction()
        {
            Console.Clear();

            if (ShowcaseRepository.ActivesCount() == 0 || ProductRepository.Count() == 0)
                return new Result() { Message = "Нет товара и витрин для отображения" };

            IResult result = new Result();

            Output.Write("Размещение товара на витрине", ConsoleColor.Yellow);

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ");

            if (int.TryParse(Console.ReadLine(), out int scId) && scId > 0)
            {
                Showcase showcase = ShowcaseRepository.GetById(scId);

                if (showcase == null || showcase.RemovedAt.HasValue)
                    return new Result() { Message = "Витрины с идентификатором " + scId + " не найдено" };

                Console.Clear();
                PrintProductsAction(false);

                Output.Write("\r\nВведите Id товара: ");
                if (int.TryParse(Console.ReadLine(), out int pId) && pId > 0)
                {
                    Product product = ProductRepository.GetById(pId);

                    if (product == null)
                        return new Result() { Message = "Товара с идентификатором " + pId + " не найдено" };

                    Output.Write("Выбран товар ");
                    Output.WriteLine(product.Name, ConsoleColor.Cyan);

                    Output.Write("Введите количество: ");
                    if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                    {
                        Output.Write("Введите стоимость: ");
                        if (int.TryParse(Console.ReadLine(), out int cost) && cost > 0)
                        {
                            return ShowcaseRepository.Place(showcase.Id, product, quantity, cost);
                        }
                        else result.Message = "Стоимость товара должна быть положительным числом";
                    }
                    else result.Message = "Количество товара должно быть положительным числом";
                }
                else result.Message = "Идентификатор товара должен быть положительным числом";
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

            if (ProductRepository.Count() == 0)
            {
                Output.WriteLine("Нет товаров для отображения");
                Console.ReadKey();
                return;
            }

            Output.WriteLine("Доступные товары", ConsoleColor.Cyan);            
            foreach (Product product in ProductRepository.All())
                Output.WriteLine(product.ToString());

            if (waitPressKey)
                Console.ReadKey();
        }

        /// <summary>
        /// Вызывает сценарий отображения всех товаров на витрине
        /// </summary>
        /// <returns></returns>
        void PrintShowcaseProductsAction(bool waitPressKey = true)
        {
            if (ShowcaseRepository.ActivesCount() == 0 || ProductRepository.Count() == 0)
            {
                Console.Clear();
                Output.WriteLine("Нет товаров и витрин для отображения");
                Console.ReadKey();
                return;
            }

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Showcase showcase = ShowcaseRepository.GetById(id);

                if (showcase != null && !showcase.RemovedAt.HasValue)
                {
                    Output.Write("\r\nТовары на витрине ");
                    Output.WriteLine(showcase.Name + ":", ConsoleColor.Cyan);

                    List<int> ids = ShowcaseRepository.GetShowcaseProductsIds(showcase);

                    if (ids.Count > 0)
                    {
                        foreach (int pId in ids)
                        {
                            Product product = ProductRepository.GetById(pId);

                            if (product != null)
                                Output.WriteLine(product.ToString());
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

            int count = 0;

            foreach (Showcase showcase in ShowcaseRepository.All())
                if ((showOnlyDeleted && showcase.RemovedAt.HasValue) || (!showOnlyDeleted && !showcase.RemovedAt.HasValue))
                {
                    Output.WriteLine(showcase.ToString());
                    count++;
                }

            if (count == 0)
                Output.WriteLine("Нет витрин для отображения");

            if (waitPressKey)
                Console.ReadKey();
        }

        #endregion

        /// <summary>
        /// Выводит на экран сообщение о результате выполнения действия
        /// </summary>
        /// <param name="result"></param>
        void ShowResult(IResult result)
        {
            if (result.Success && string.IsNullOrWhiteSpace(result.Message))
                result.Message = "Выполнено";

            Output.WriteLine(result.Message, result.Success ? ConsoleColor.Green : ConsoleColor.Red);

            Output.WriteLine("Нажмите любую клавишу для продолжения..", ConsoleColor.Yellow);
            Console.ReadKey();
        }
    }
}
