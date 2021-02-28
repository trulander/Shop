using Shop.Model;
using System.Collections.Generic;

namespace Shop.DAL
{
    interface IProductRepository : IRepository<Product>
    {
        int ProductsCapacity(List<ProductShowcase> productsShowcase);
    }
}
