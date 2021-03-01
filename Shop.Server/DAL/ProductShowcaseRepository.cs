using Shop.Model;
using System;
using System.Collections.Generic;

namespace Shop.Server.DAL
{
    class ProductShowcaseRepository : IProductShowcaseRepository
    {
        List<ProductShowcase> _items = new List<ProductShowcase>();
        int _lastInsertedId = 0;

        public void Add(ProductShowcase entity)
        {
            entity.Id = ++_lastInsertedId;
            _items.Add(entity);
        }

        public IEnumerable<ProductShowcase> All()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public ProductShowcase GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Seed(int count)
        {
            throw new NotImplementedException();
        }

        public void Update(ProductShowcase entity)
        {
            throw new NotImplementedException();
        }
    }
}
