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
    public partial class AddNewPersonDialog : DialogWindowBase
    {
        public AddNewPersonDialog(long initialId, bool teacher)
        {
            Description = "新增" + (teacher ? "教师" : "学生");
            Person.Role = teacher ? "教师" : "学生";

            Person.Id = initialId;
            InitializeComponent();
            tbkId.Text = teacher ? "工号" : "学号"; if (teacher)
            {
                grd.RowDefinitions[6].Height =
                    grd.RowDefinitions[7].Height =
                    grd.RowDefinitions[8].Height =
                    grd.RowDefinitions[9].Height = new GridLength(0);
            }
        }
        public string Description { get; private set; }
        public Person Person { get; private set; } = new Person() { Password = "123456", Gender = 0, Born = new DateTime(2000, 1, 1) };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Person.Name))
            {
                TaskDialog.ShowError(this, "姓名不可为空");
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
