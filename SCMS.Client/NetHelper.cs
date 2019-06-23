using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCMS.Client
{
    public static class NetHelper
    {
        /// <summary>
        /// 向服务器发送对象并返回接受的Json对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async static Task<JObject> Send(object obj)
        {
            return JObject.Parse(await Send(JsonConvert.SerializeObject(obj)));
        }
        /// <summary>
        /// 向服务器发送对象，并返回接受的对象。接收时暂时禁用按钮
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public async static Task<JObject> Send(object obj, FrameworkElement button)
        {
            button.IsEnabled = false;
            var result = await Send(JsonConvert.SerializeObject(obj));
            button.IsEnabled = true;
            return JObject.Parse(result);
        }
        /// <summary>
        /// 像服务器发送信息并返回接受的信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<string> Send(string message)
        {
            return Task.Run(() =>
             {
                 //设定服务器IP地址  
                 IPAddress ip = IPAddress.Parse(Config.Instance.ServerIP);
                 Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                 try
                 {
                     //尝试连接服务器
                     clientSocket.Connect(new IPEndPoint(ip, Config.Instance.ServerPort)); //配置服务器IP与端口  
                 }
                 catch
                 {
                     //返回错误信息
                     return JsonConvert.SerializeObject(new { OK = false, Message = $"无法连接到服务器{Config.Instance.ServerIP}:{Config.Instance.ServerPort}" });
                 }
                 //向服务器发送数据
                 clientSocket.Send(Encoding.UTF8.GetBytes(message));

                 //接受从服务器返回的信息
                 byte[] buffer = new byte[1024 * 1024 * 5];
                 int length = clientSocket.Receive(buffer, buffer.Length, 0);
                 string str = Encoding.UTF8.GetString(buffer, 0, length);
                 clientSocket.Close();
                 return str;
             });
        }

    }
}
