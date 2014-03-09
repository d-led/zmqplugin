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

    static class Extensions
    {
        public static byte[] ToByteArray(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ToStringFromByteArray(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }

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
                            var message = "bla".ToByteArray();
                            var reply = my_plugin.call(message);
                            Console.WriteLine("Echo ok: {0}", reply.SequenceEqual(message));
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
