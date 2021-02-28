using System.Collections.Generic;

namespace Shop.Model
{
    public interface IContainerMenuItem: IMenuItem
    {
        public List<IMenuItem> Children { get; set; }
    }
}
