using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace MagicLibrary.MVC.WinForms
{
    [Serializable]
    public class WFControlView : View
    {
        public Control Control { get; set; }

        public WFControlView(Control control)
        {
            this.Control = control;
        }
        
        public override void Refresh()
        {
            Control.Refresh();
        }
    }
}
