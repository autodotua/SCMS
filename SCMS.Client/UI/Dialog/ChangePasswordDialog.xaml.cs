using FzLib.Control.Dialog;
using FzLib.Cryptography;
using Newtonsoft.Json.Linq;
using SCMS.Common;
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
    public partial class ChangePasswordDialog : DialogWindowBase
    {
        Person person;
        public ChangePasswordDialog(Person person)
        {
            Description = $"修改用户{person.Name}的密码";
            this.person = person;
            InitializeComponent();
        }
        public string Description { get; private set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwbOld.Password)
                     || string.IsNullOrEmpty(pwbNew1.Password)
                     || string.IsNullOrEmpty(pwbNew2.Password))
            {
                TaskDialog.ShowError(this, "请填写所有密码框");
                return;
            }
            if (pwbNew1.Password != pwbNew2.Password)
            {
                TaskDialog.ShowError(this, "两次密码不完全相同");
                return;
            }
            Hash hash = new Hash();
            string OldPassword = hash.GetString("MD5", pwbOld.Password);
            string NewPassword = hash.GetString("MD5", pwbNew1.Password);

            JObject json = await NetHelper.Send(new {Command=ApiCommands.ChangePassword, ID = person.Id, OldPassword, NewPassword });
            string message = json[ApiCommands.Message].Value<string>();
            if (json[ApiCommands.OK].Value<bool>())
            {
                TaskDialog.Show(this, message);
                DialogResult = true;
                Close();
            }
            else
            {
                TaskDialog.ShowError(this,message);

            }
        }
    }
}
