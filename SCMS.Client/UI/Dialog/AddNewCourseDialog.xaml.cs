using FzLib.Control.Dialog;
using SCMS.Common.DbData;
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

namespace SCMS.Client.UI.Dialog
{
    /// <summary>
    /// AddNewPersonDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddNewCourseDialog : DialogWindowBase
    {
        public AddNewCourseDialog(int initialId, string description)
        {
            Description = description;
            Course.Id = initialId;
            InitializeComponent();
        }
        public string Description { get; private set; }
        public Course Course { get; private set; } = new Course() { Year = (short)DateTime.Now.Year ,Term=1,Credit=1 };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Course.Name))
            {
                TaskDialog.ShowError(this, "课程名不可为空");
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
