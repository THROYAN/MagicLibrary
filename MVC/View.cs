using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MVC
{
    [Serializable]
    public class View : IView
    {
        public List<IController> Controllers { get; set; }

        public View()
        {
            Controllers = new List<IController>();
        }

        public void AddController(IController controller)
        {
            Controllers.Add(controller);
        }

        public virtual IController GetController(string name)
        {
            return Controllers.Find(controller => controller.Name == name);
        }

        public virtual void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
