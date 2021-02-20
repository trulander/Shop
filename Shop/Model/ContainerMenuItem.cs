using System.Collections.Generic;

namespace Shop.Model
{
    class ContainerMenuItem: MenuItem
    {
        public List<IMenuItem> Children { get; set; }

        public ContainerMenuItem(string text, IEnumerable<IMenuItem> children = null):base(text)
        {
            Children = new List<IMenuItem>();

            foreach (IMenuItem item in children)
            {
                item.Parent = this;
                Children.Add(item);
            }
        }
    }
}
