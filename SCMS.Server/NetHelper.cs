using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static SCMS.Common.ApiCommands;

namespace SCMS.Server
{
    public static class NetHelper
    {
        /// <summary>
        /// socket对象
        /// </summary>
        private static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// <summary>
        /// 接收信息的缓存
        /// </summary>
        private static byte[] buffer = new byte[1024 * 1024];
        /// <summary>
        /// 监听线程
        /// </summary>
        private static Thread thread;
        /// <summary>
        /// 打开监听线程
        /// </summary>
        public static void SocketServie()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                (Application.Current.MainWindow as MainWindow).AddLog("服务器开启", "");
            });
            string host = Config.Instance.DeviceIP;//IP地址
            int port = Config.Instance.Port;//端口
            socket.Bind(new IPEndPoint(IPAddress.Parse(host), port));
            socket.Listen(100);//设定最多100个排队连接请求   
            thread = new Thread(() =>
            {
                while (true)
                {
                    Socket clientSocket = null;
                    try
                    {
                        clientSocket = socket.Accept();
                    }
                    catch
                    {

                    }
                    if (closing)
                    {
                        return;
                    }
                    Thread receiveThread = new Thread(ReceiveMessage);
                    receiveThread.Start(clientSocket);
                }
            });//通过多线程监听客户端连接  
            thread.Start();
        }


        /// <summary>  
        /// 接收消息线程方法
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private static void ReceiveMessage(object clientSocketObj)
        {
            Socket clientSocket = (Socket)clientSocketObj;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = clientSocket.Receive(buffer);
                    if (receiveNumber == 0)
                        return;
                    string message = Encoding.UTF8.GetString(buffer, 0, receiveNumber);
                    string ip = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();
                    string sendStr = GetReturnMessage(message, ip);
                    //给Client端返回信息
                    byte[] bs = Encoding.UTF8.GetBytes(sendStr);//Encoding.UTF8.GetBytes()不然中文会乱码

                    clientSocket.Send(bs, bs.Length, 0);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        (Application.Current.MainWindow as MainWindow).AddLog("服务器发生异常，已退出", "");
                    });
                    Debug.WriteLine(ex.Message);
                    clientSocket.Shutdown(SocketShutdown.Both);//禁止发送和上传
                    clientSocket.Close();//关闭Socket并释放资源
                    break;
                }
            }
        }

        /// <summary>
        /// 根据Command来确定需要返回的信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GetReturnMessage(string message, string ip)
        {
            string sendStr = null;
            try
            {
                //解析发送的Json字符串
                JObject json = JObject.Parse(message);


                string command = json[Command].Value<string>();
                if (command == Login)
                {
                    sendStr = DatabaseHelper.Instance.Login(json);
                }
                else if (command == Admin_Students)
                {
                    sendStr = DatabaseHelper.Instance.GetAllStudents();
                }
                else if (command == Admin_Teachers)
                {
                    sendStr = DatabaseHelper.Instance.GetAllTeachers();
                }
                else if (command == Admin_Courses)
                {
                    sendStr = DatabaseHelper.Instance.GetAllCourses();
                }
                else if (command == Teacher_Courses)
                {
                    sendStr = DatabaseHelper.Instance.GetTeacherCoursesList(json);
                }
                else if (command == Teacher_StudentScoreList)
                {
                    sendStr = DatabaseHelper.Instance.GetStudentsScoresList(json);
                }
                else if (command == Student_Courses)
                {
                    sendStr = DatabaseHelper.Instance.GetStudentCoursesList(json);
                }
                else if (command == Admin_Update_Persons)
                {
                    sendStr = DatabaseHelper.Instance.UpdatePerson(json);
                }
                else if (command == Admin_Update_Courses)
                {
                    sendStr = DatabaseHelper.Instance.UpdateCourse(json);
                }

                else if (command == Admin_CourseOfStudent)
                {
                    sendStr = DatabaseHelper.Instance.GetCoursesOfStudent(json);
                }

                else if (command == Admin_CourseOfTeacher)
                {
                    sendStr = DatabaseHelper.Instance.GetCourseOfTeacher(json);
                }

                else if (command == Admin_StudentsAndTeacherOfCourses)
                {
                    sendStr = DatabaseHelper.Instance.GetStudentsAndTeacherOfCourse(json);
                }

                else if (command == Admin_SetStudentsAndTeacherOfCourse)
                {
                    sendStr = DatabaseHelper.Instance.SetStudentsAndTeacherOfCourse(json);
                }
                else if (command == Teacher_UpdateStudentScore)
                {
                    sendStr = DatabaseHelper.Instance.UpdateStudentsScore(json);
                }

                else if (command == ChangePassword)
                {
                    sendStr = DatabaseHelper.Instance.ChangePassword(json);
                }

                else
                {
                    sendStr = JsonConvert.SerializeObject(new { OK = false, Message = "未知API" });
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    (Application.Current.MainWindow as MainWindow).AddLog("接收到客户端请求，类型：" + command, ip);
                });
            }
            catch (Exception ex)
            {
                Debug.Assert(false);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                sendStr = JsonConvert.SerializeObject(new { OK = false, ex.Message });
                Application.Current.Dispatcher.Invoke(() =>
                {
                    (Application.Current.MainWindow as MainWindow).AddLog("接收到客户端请求，但运行错误", ip);
                });
            }
            return sendStr;
        }

        /// <summary>
        /// 服务是否正在关闭
        /// </summary>
        private static bool closing = false;
        /// <summary>
        /// 关闭Socket
        /// </summary>
        public static void Close()
        {

            closing = true;
            socket.Close();
            socket.Dispose();
        }
    }
}
