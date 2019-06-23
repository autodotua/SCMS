using System;

namespace SCMS.Server
{
    public class LogInfo
    {
        public LogInfo(string content,string ip,string command,string send)
        {
            Time = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
            IP = ip;
            Content = content;
            Receive = command;
            Send = send;
        }
        public string IP { get; }
        public string Time { get; }
        public string Content { get; }
        public string Receive { get; }
        public string Send { get; }
    }
}
