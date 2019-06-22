using FzLib.Control.Dialog;
using Newtonsoft.Json;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCMS.Client.UI
{
    /// <summary>
    /// PageLogin.xaml 的交互逻辑
    /// </summary>
    public partial class PageLogin : Page
    {
        FzLib.Cryptography.Hash md5 = new FzLib.Cryptography.Hash();
        public PageLogin()
        {
            if(App.Example)
            {
                Config.ServerIP = "47.101.216.232";
                Config.ServerPort = 8008;
                Config.UserId = 1;
                Config.UserPassword = "000000";

            }
            InitializeComponent();
            pswd.Password = Config.Instance.UserPassword;
        }
        public Config Config => Config.Instance;
        /// <summary>
        /// 单击登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            //使用MD5加密密码
            Config.Instance.UserPassword = pswd.Password.Length == 32 ? pswd.Password : md5.GetString("MD5", pswd.Password);
            JObject jsonRecv = await NetHelper.Send(new { Command = ApiCommands.Login, ID = Config.UserId, Password = Config.UserPassword }) as JObject;
            if (jsonRecv[ApiCommands.OK].Value<bool>())
            {
                //登录界面渐隐
                DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3), FillBehavior.Stop);
                ani.Completed += (p1, p2) =>
                {
                    grd.Opacity = 0;
                    grd.Children.Clear();
                    //向grd添加“登陆成功”的标签
                    TextBlock tbk = new TextBlock() { FontSize = 28, Text = "登录成功", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                    grd.Children.Add(tbk);
                    Grid.SetColumnSpan(tbk, 1000);
                    Grid.SetRowSpan(tbk, 1000);
                    //登陆成功渐显
                    DoubleAnimation ani2 = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3), FillBehavior.Stop);
                    ani2.Completed += async (p3, p4) =>
                     {
                         grd.Opacity = 1;
                         await Task.Delay(1000);
                         //登陆成功渐隐
                         DoubleAnimation ani3 = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3), FillBehavior.HoldEnd);
                         ani3.Completed += (p5, p6) =>
                          {
                              Person person = jsonRecv[ApiCommands.Person].ToObject<Person>();

                              switch (person.Role)
                              {
                                  case "学生":
                                      var page1 = new PageStudent(person);
                                      (Window.GetWindow(this) as MainWindow).CurrentPage = page1;
                                      NavigationService.Navigate(page1);
                                      break;
                                  case "管理员":
                                      var page2 = new PageAdmin(person);
                                      (Window.GetWindow(this) as MainWindow).CurrentPage = page2;
                                      NavigationService.Navigate(page2);
                                      break;
                                  case "教师":
                                      var page3 = new PageTeacher(person);
                                      (Window.GetWindow(this) as MainWindow).CurrentPage = page3;
                                      NavigationService.Navigate(page3);
                                      break;
                                  default:
                                      TaskDialog.Show("未知角色");
                                      break;
                              }
                               (Window.GetWindow(this) as MainWindow).CurrentUser = person;
                          };
                         grd.BeginAnimation(OpacityProperty, ani3);
                     };
                    grd.BeginAnimation(OpacityProperty, ani2);

                };
                grd.BeginAnimation(OpacityProperty, ani);

            }
            else
            {
                //登陆失败，显示错误信息
                TaskDialog.ShowError(jsonRecv[ApiCommands.Message].Value<string>());
                btnLogin.IsEnabled = true;

            }
            Config.Save();
        }

    }
}
