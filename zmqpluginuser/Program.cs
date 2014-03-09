using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zmqpluginuser
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new config
            {
                dll = "zmqplugin.dll",
                protocol = "msgpack",
                bind_string = "tcp://*:5556",
                connect_string = "tcp://localhost:5556"
            };
            try
            {
                using (var my_plugin = new zmq_plugin(cfg.dll))
                {
                    var load_result = my_plugin.load(JsonConvert.SerializeObject(cfg));
                    switch (load_result)
                    {
                        case zmqpluginuser.load_result.OK:
                            Console.WriteLine("Hello {0}", my_plugin.hello());
                            break;
                        case zmqpluginuser.load_result.FAILED_CONNECTING:
                            Console.WriteLine("failed connecting");
                            break;
                        case zmqpluginuser.load_result.FAILED_LOADING:
                            Console.WriteLine("failed loading");
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
