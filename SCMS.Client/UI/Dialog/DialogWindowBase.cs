using FzLib.Control.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCMS.Client.UI.Dialog
{
  public  class DialogWindowBase : ExtendedWindow
    {
        public DialogWindowBase()
        {
            Owner = MainWindow.Instance;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
            SizeToContent = SizeToContent.WidthAndHeight;
            ShowInTaskbar = false;
        }
    }
}
