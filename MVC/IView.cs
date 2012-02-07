using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MVC
{
    public interface IView
    {
        List<IController> Controllers { get; set; }

        void AddController(IController controller);

        IController GetController(string name);

        void Refresh();
    }
}
