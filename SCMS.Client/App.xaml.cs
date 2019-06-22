using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SCMS.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            if(e.Args.Length>0 && e.Args[0]==nameof(Example))
            {
                Example = true;
            }
            FzLib.Program.Runtime.UnhandledException.RegistAll();
        }
        public static bool Example { get; private set; }//示例模式，自动填充服务器地址
    }
    
}
