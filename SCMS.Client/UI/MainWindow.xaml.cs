using FzLib.Control.Dialog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCMS.Client.UI;
using SCMS.Client.UI.Dialog;
using SCMS.Common.DbData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

namespace SCMS.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            TaskDialog.DefaultOwner = this;
            frame.Navigated += (p1, p2) =>
              {
                  if (!(p2.Content is PageLogin))
                  {
                      btnLogout.Visibility = Visibility.Visible;
                      btnChangePassword.Visibility = Visibility.Visible;
                  }
              };
        }

        bool restart = false;
        private void LogoutButtonClick(object sender, RoutedEventArgs e)
        {
            restart = true;
            Close();
        }
        public IPageClosing CurrentPage { get; set; } = null;
        public Person CurrentUser { get; set; } = null;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CurrentPage != null)
            {
                if (!CurrentPage.PageClosing())
                {
                    e.Cancel = true;
                    restart = false;
                }
            }
            if (restart)
            {
                new MainWindow().Show();
            }
        }

        private void ChangePasswordButtonClick(object sender, RoutedEventArgs e)
        {
            new ChangePasswordDialog(CurrentUser).ShowDialog();
        }
    }
}
