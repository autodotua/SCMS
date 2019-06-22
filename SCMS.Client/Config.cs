using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMS.Client
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
                    instance = TryOpenOrCreate<Config>("ClientConfig.json");
                }
                return instance;
            }
        }

        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 8008;

        public long UserId { get; set; } = 201718110101;
        public string UserPassword { get; set; } = "000000";
    }
}
