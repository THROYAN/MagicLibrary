using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MVC
{
    public interface IController
    {
        IView View { get; set; }

        string Name { get; set; }
    }
}
