using FzLib.Control.Dialog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCMS.Common.DbData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCMS.Server
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public Config Config => Config.Instance;
        public ObservableCollection<LogInfo> Logs { get; } = new ObservableCollection<LogInfo>();
        public MainWindow()
        {
            InitializeComponent();
            TaskDialog.DefaultOwner = this;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Config.Save();
            if(string.IsNullOrWhiteSpace(Config.DbConnectionString))
            {
                TaskDialog.ShowError("数据库连接字符串为空！");
                return;
            }
            grdControl.IsEnabled = false;
            btnStart.Content = "正在启动服务";
            try
            {
                DatabaseHelper.Instance.Model = new SCMSModel(Config.DbConnectionString);
                DatabaseHelper.Instance.Model.Database.CommandTimeout = 2;

                bool exist = false;
                await Task.Run(() =>
                {
                    exist = DatabaseHelper.Instance.Model.Database.Exists();
                });
                if (!exist)
                {
                    TaskDialog.ShowError("数据库连接失败");
                    grdControl.IsEnabled = true;
                    btnStart.Content = "启动";
                    return;

                }
                //model.Database.Exists();

                NetHelper.SocketServie();
                btnStart.Content = "服务已启动";
            }
            catch (Exception ex)
            {
                TaskDialog.ShowException(ex, "启动失败");
                try
                {
                    DatabaseHelper.Instance.Model.Dispose();
                }
                catch
                {

                }
                grdControl.IsEnabled = true;
                btnStart.Content = "启动";
            }
        }




        public void AddLog(string content,string ip, string receive,string send)
        {
            LogInfo log = new LogInfo(content,ip,receive,send);
            Logs.Add(log);
            lvwLog.ScrollIntoView(log);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NetHelper.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
//#if DEBUG
//            Button_Click(null, null);
//            WindowState = WindowState.Minimized;
//#endif
        }

        private void LvwLog_ItemPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lvwLog.SelectedItem!=null &&!string.IsNullOrEmpty((lvwLog.SelectedItem as LogInfo).Receive))
            {
                LogInfo log = lvwLog.SelectedItem as LogInfo;
                new LogDetailWindow(log.Receive, log.Send) { Owner = this }.Show();
            }
        }
    }
}
