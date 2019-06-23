using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCMS.Server
{
    /// <summary>
    /// LogDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogDetailWindow : Window
    {
        public LogDetailWindow(string receive,string send)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            rt1.Document.Blocks.Add(new Paragraph(new Run(receive)));
            rt2.Document.Blocks.Add(new Paragraph(new Run(send)));
        }

    }
}
