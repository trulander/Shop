using Shop.Model;
using System.Collections.Generic;

namespace Shop.Server.DAL
{
    internal interface IShowcaseRepository: IRepository<Showcase>
    {
        int ActivesCount();
        int RemovedCount();

        void TakeOut(Product product);
        List<int> GetShowcaseProductsIds(Showcase showcase);
        List<ProductShowcase> GetShowcaseProducts(Showcase showcase);

        IResult Place(int showcaseId, Product product, int quantity, decimal cost);
    }
}