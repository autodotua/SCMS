using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMS.Server
{
    public class Config : FzLib.DataStorage.Serialization.JsonSerializationBase
    {
        private static Config instance;
        public static Config Instance
        {
            get
            {
                if(instance==null)
                {
                    instance = TryOpenOrCreate<Config>("ServerConfig.json");
                }
                return instance;
            }
        }
        public string DbConnectionString { get; set; } = "";//@"Data Source=FZ-LAPTOP\SQLEXPRESS;Initial Catalog=SCMS;Integrated Security=True";
        public string DeviceIP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8008;
    }
}
