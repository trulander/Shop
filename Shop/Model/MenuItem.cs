namespace Shop.Model
{
    abstract class MenuItem: IMenuItem
    {
        public ContainerMenuItem Parent { get; set; }
        public string Text { get; set; }

        public MenuItem(string text)
        {
            Text = text;
        }

        public string GetFullPathText()
        {
            string full = Parent?.GetFullPathText();

            if (!string.IsNullOrWhiteSpace(full))
                full += " \u00BB ";

            return full + Text;
        }

        public override string ToString() => Text;
    }
}
