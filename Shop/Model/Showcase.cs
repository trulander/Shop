using System;
using System.Collections.Generic;

namespace Shop.Model
{
    class Showcase : Entity
    {
        public string Name { get; set; }
        public int MaxCapacity { get; set; }
        public int Capacity { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RemovedAt { get; set; } = null;

        private List<ProductShowcase> _productShowcases { get; set; }
        private int _productShowcasesLastInsertedId = 0;

        public Showcase()
        {
            _productShowcases = new List<ProductShowcase>();
        }

        public override IValidateResult Validate()
        {
            IValidateResult result = new ValidateResult(true);

            if (string.IsNullOrWhiteSpace(Name))
            {
                result.Success = false;
                result.Message += "Наименование не должно быть пустым\r\n";
            }

            if (MaxCapacity < 1)
            {
                result.Success = false;
                result.Message += "Максимально допустимый объем должен быть целым положительным числом\r\n";
            }

            return result;
        }

        public IValidateResult ProductPlace(Product product, int quantity, decimal cost)
        {
            IValidateResult result = new ValidateResult(false);
            ProductShowcase ps = new ProductShowcase(Id, product.Id, quantity, cost);

            IValidateResult validateResult = ps.Validate();

            if (validateResult.Success)
            {
                if (Capacity + (product.Capacity * quantity) <= MaxCapacity)
                {
                    ps.Id = ++_productShowcasesLastInsertedId;
                    _productShowcases.Add(ps);
                    result.Success = true;
                }
                else result.Message = "Объем витрины не позволяет разместить товар";
            }
            else result.Message = validateResult.Message;
            
            return result;
        }

        public List<int> GetProductsIds()
        {
            List<int> ids = new List<int>();
            foreach (ProductShowcase psc in _productShowcases)
                ids.Add(psc.ProductId);

            return ids;
        }

        public bool HasProduct(int id)
        {
            bool result = false;
            foreach (ProductShowcase psc in _productShowcases)
            {
                if (psc.ProductId.Equals(id))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void ProductRemove(int id)
        {
            for (int i = 0; i < _productShowcases.Count; i++)
            {
                if (_productShowcases[i].ProductId.Equals(id))
                {
                    _productShowcases.RemoveAt(i);
                    break;
                }
            }
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} (max: {MaxCapacity}) от " + CreatedAt.ToShortDateString() + ((RemovedAt != null) ? "(удалено " + RemovedAt?.ToShortDateString() + ")" : "");
        }

        internal int GetProductsCapacity(IEnumerable<Product> products)
        {
            int capacity = 0;

            for (int i = 0; i < _productShowcases.Count; i++)
                foreach (Product product in products)
                    if (_productShowcases[i].ShowcaseId == Id && _productShowcases[i].ProductId.Equals(product.Id))
                    {
                        capacity += product.Capacity * _productShowcases[i].Quantity;
                        break;
                    }

            return capacity;
        }
    }
}