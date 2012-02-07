using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MVC
{
    public class Controller : IController
    {
        public IView View { get; set; }
        public string Name { get; set; }

        public Controller(string name)
        {
            Name = name;
        }
    }
}
