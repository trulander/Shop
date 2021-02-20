using System.Collections.Generic;

namespace Shop.Model
{
    class ProductRepository : IProductRepository
    {
        List<Product> _items = new List<Product>();
        int _lastInsertedId = 0;

        public int Count() => _items.Count;
        public IEnumerable<Product> All() => _items;

        public void Add(Product entity)
        {
            entity.Id = ++_lastInsertedId;
            _items.Add(entity);
        }

        public Product GetById(int id)
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
                    _items.RemoveAt(i);
                    break;
                }
            }
        }

        public void Update(Product entity)
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
            for (int i = 0; i < count; i++)
            {
                _items.Add(new Product()
                {
                    Id = ++_lastInsertedId,
                    Name = "Товар " + (i + 1),
                    Capacity = 1 + i
                });
            }
        }

        
    }
}
