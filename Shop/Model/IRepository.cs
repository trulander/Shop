using System.Collections.Generic;

namespace Shop.Model
{
    interface IRepository<T>
    {
        void Add(T entity);
        void Update(T entity);
        void Remove(int id);
        T GetById(int id);
        IEnumerable<T> All();
        int Count();
        void Seed(int count);
    }
}
