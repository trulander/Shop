using Shop.Model;

namespace Shop.Controller
{
    class MenuController
    {
        public IContainerMenuItem Current { get; private set; }
        public int SelectedIndex { get; private set; } = 0;

        public MenuController(IContainerMenuItem root)
        {
            Current = root;
        }

        public void Prev()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
            else
                SelectedIndex = (Current.Children.Count > 0) ? Current.Children.Count - 1 : 0;
        }
        public void Next()
        {
            if (SelectedIndex < Current.Children.Count - 1)
                SelectedIndex++;
            else
                SelectedIndex = 0;
        }
        public void Return()
        {
            if (SelectedIndex > 0)
                SelectedIndex = 0;
            else
                if (Current.Parent != null)
                    Current = Current.Parent;
        }

        public void Expand(IContainerMenuItem container)
        {
            Current = container;
            SelectedIndex = 0;
        }
    }
}
