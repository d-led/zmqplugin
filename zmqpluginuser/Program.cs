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
                Console.WriteLine(Path.GetFullPath("."));
                Console.WriteLine(File.Exists("zmqplugin.dll"));
                using (var my_plugin = new zmq_plugin("zmqplugin.dll"))
                {
                    var load_result = my_plugin.load(JsonConvert.SerializeObject(
                        new config
                        {
                            protocol = "msgpack",
                            plugin_connection_string = "tcp://localhost:5555"
                        }
                       ));
                    if (load_result == zmqpluginuser.load_result.OK)
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
