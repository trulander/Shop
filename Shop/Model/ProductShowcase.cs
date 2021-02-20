namespace Shop.Model
{
    class ProductShowcase: Entity
    {
        public int ShowcaseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }

        public override IResult Validate()
        {
            IResult result = new Result(true);

            if (ShowcaseId < 1)
            {
                result.Success = false;
                result.Message += "Идентификатор витрины должен быть целым положительным числом\r\n";
            }

            if (ProductId < 1)
            {
                result.Success = false;
                result.Message += "Идентификатор товара должен быть целым положительным числом\r\n";
            }

            if (Quantity < 1)
            {
                result.Success = false;
                result.Message += "Количество товара должно быть целым положительным числом\r\n";
            }

            if (Cost < 1)
            {
                result.Success = false;
                result.Message += "Стоимость товара должна быть целым положительным числом\r\n";
            }

            return result;
        }

        public ProductShowcase(int scId, int pId, int quantity, decimal cost)
        {
            ShowcaseId = scId;
            ProductId = pId;
            Quantity = quantity;
            Cost = cost;
        }
    }
}
