using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MagicLibrary.MVC;

namespace MagicLibrary.MVC.WinForms
{
    public class WinFormView : Form, IView
    {
        public List<IController> Controllers { get; set; }

        public void AddController(IController controller)
        {
            Controllers.Add(controller);
        }

        public IController GetController(string name)
        {
            return Controllers.Find(controller => controller.Name == name);
        }

        public WinFormView()
        {
            Controllers = new List<IController>();
        }

        public override void Refresh()
        {
            base.Refresh();
        }
    }
}
