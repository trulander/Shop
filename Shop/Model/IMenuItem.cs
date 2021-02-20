namespace Shop.Model
{
    interface IMenuItem
    {
        public ContainerMenuItem Parent { get; set; }
        public string Text { get; set; }
    }
}
