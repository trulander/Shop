namespace Shop.Model
{
    public abstract class MenuItem: IMenuItem
    {
        public IContainerMenuItem Parent { get; set; }
        public string Text { get; set; }

        public MenuItem(string text)
        {
            Text = text;
        }

        public string GetFullPathText()
        {
            var full = Parent?.GetFullPathText();

            if (!string.IsNullOrWhiteSpace(full))
                full += " \u00BB ";

            return full + Text;
        }
        public override string ToString() => Text;
    }
}
