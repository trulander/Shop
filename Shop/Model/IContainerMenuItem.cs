using System.Collections.Generic;

namespace Shop.Model
{
    interface IContainerMenuItem: IMenuItem
    {
        public List<IMenuItem> Children { get; set; }
    }
}
