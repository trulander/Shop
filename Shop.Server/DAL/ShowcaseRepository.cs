using Shop.Model;
using System;
using System.Collections.Generic;

namespace Shop.Server.DAL
{
    class ShowcaseRepository : IShowcaseRepository
    {
        readonly List<Showcase> _items = new List<Showcase>();
        readonly List<ProductShowcase> _products = new List<ProductShowcase>();

        int _lastInsertedId = 0;
        int _lastProductInsertedId = 0;

        public int Count() => _items.Count;
        public IEnumerable<Showcase> All() => _items;

        public int ActivesCount()
        {
            var count = 0;
            foreach (var item in _items)
                if (!item.RemovedAt.HasValue)
                    count++;
            return count;
        }

        public int RemovedCount()
        {
            var count = 0;
            foreach (var item in _items)
                if (item.RemovedAt.HasValue)
                    count++;
            return count;
        }

        public void Add(Showcase entity)
        {
            entity.Id = ++_lastInsertedId;
            entity.CreatedAt = DateTime.Now;
            _items.Add(entity);
        }

        public Showcase GetById(int id)
        {
            for (int i = 0; i < _items.Count; i++)
                if (_items[i].Id.Equals(id))
                    return _items[i];

            return null;
        }

        public void Remove(int id)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id.Equals(id))
                {
                    _items[i].RemovedAt = DateTime.Now;
                    break;
                }
            }
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

            if (GetShowcaseProductsIds(showcase).Count > 0)
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

        public List<int> GetShowcaseProductsIds(Showcase showcase)
        {
            var ids = new List<int>();
            foreach (var psc in _products)
                if (showcase.Id == psc.ShowcaseId)
                    ids.Add(psc.ProductId);

            return ids;
        }
        public void TakeOut(Product product)
        {
            for (var i = 0; i < _products.Count; i++)
            {
                if (_products[i].ProductId.Equals(product.Id))
                {
                    _products.RemoveAt(i);
                    break;
                }
            }
        }

        public List<ProductShowcase> GetShowcaseProducts(Showcase showcase)
        {
            var result = new List<ProductShowcase>();
            foreach (var psc in _products)
                if (showcase.Id == psc.ShowcaseId)
                    result.Add(psc);

            return result;
        }
    }
}
