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
            try
            {
                using (var my_plugin = new zmq_plugin("zmqplugin.dll"))
                {
                    int load_result = my_plugin.load(JsonConvert.SerializeObject(
                        new config
                        {
                            protocol = "msgpack",
                            plugin_connection_string = "tcp://*:5555"
                        }
                       ));
                    if (load_result == 0)
                    {
                        Console.WriteLine("loaded correctly");
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
