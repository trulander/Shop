using Shop.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.DAL
{
    class ShowcaseRepository : IShowcaseRepository
    {
        readonly List<Showcase> _items = new List<Showcase>();
        readonly List<ProductShowcase> _products = new List<ProductShowcase>();

        int _lastInsertedId = 0;
        int _lastProductInsertedId = 0;

        public int Count() => _items.Count;
        public IEnumerable<Showcase> All() => _items;

        public int ActivesCount() => _items.Count(x => !x.RemovedAt.HasValue);
        public int RemovedCount() => _items.Count(x => x.RemovedAt.HasValue);

        public void Add(Showcase entity)
        {
            entity.Id = ++_lastInsertedId;
            entity.CreatedAt = DateTime.Now;
            _items.Add(entity);
        }

        public Showcase GetById(int id) => _items.Find(x => x.Id == id);

        public void Remove(int id)
        {
            _items
                .Where(x => x.Id == id)
                .Select(x =>
                {
                    x.RemovedAt = DateTime.Now;
                    return x;
                });
        }

        public void Update(Showcase entity)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id.Equals(entity.Id))
                {
                    _items[i] = entity;
                    break;
                }
            }
        }

        public void Seed(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _items.Add(new Showcase()
                {
                    Id = ++_lastInsertedId,
                    Name = "Витрина " + (i + 1),
                    CreatedAt = DateTime.Now,
                    MaxCapacity = 1 + i
                });
            }
        }

        public IResult Place(int showcaseId, Product product, int quantity, decimal cost)
        {
            var showcase = GetById(showcaseId);

            if (showcase == null)
                return new Result("Витрина с идентификатором " + showcaseId + " не найдена");

            if (Enumerable.Count(GetShowcaseProductsIds(showcase)) > 0)
                return new Result("Витрина уже содержит товар с указанным идентификатором");

            if (showcase.Capacity + (product.Capacity * quantity) > showcase.MaxCapacity)
                return new Result("Объем витрины не позволяет разместить товар");

            var ps = new ProductShowcase(showcaseId, product.Id, quantity, cost)
            {
                Id = ++_lastProductInsertedId
            };

            var validate = ps.Validate();

            if (validate.Success)
            {
                _products.Add(ps);
                return new Result(true);
            }
            else
                return validate;
        }

        public IEnumerable<int> GetShowcaseProductsIds(Showcase showcase)
        {
            return _products
                .Where(x => x.ShowcaseId == showcase.Id)
                .Select(x => x.ProductId);
        }
        public void TakeOut(Product product) => _products.RemoveAll(x => x.ProductId == product.Id);

        public IEnumerable<ProductShowcase> GetShowcaseProducts(Showcase showcase)
        {
            return _products
                .Where(x => x.ShowcaseId == showcase.Id)
                .ToList();
        }
    }
}
