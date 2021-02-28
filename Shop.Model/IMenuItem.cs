namespace Shop.Model
{
    public interface IMenuItem
    {
        public IContainerMenuItem Parent { get; set; }
        public string Text { get; set; }
        public string GetFullPathText();
    }
}
