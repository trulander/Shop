using System.Collections.Generic;

namespace Shop.Model
{
    class ContainerMenuItem: MenuItem, IContainerMenuItem
    {
        public List<IMenuItem> Children { get; set; } = new List<IMenuItem>();

        public ContainerMenuItem(string text, IEnumerable<IMenuItem> children = null):base(text)
        {
            foreach (var item in children)
            {
                item.Parent = this;
                Children.Add(item);
            }
        }
    }
}
