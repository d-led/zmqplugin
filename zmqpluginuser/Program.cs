using MsgPack.Serialization;
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

        public static byte[] PackMsg<T>(T t)
        {
            var serializer = MessagePackSerializer.Create<T>();
            return serializer.PackSingleObject(t);
        }

        public static byte[] Pack<T>(this T t)
        {
            return PackMsg(t);
        }

        public static T UnpackMsg<T>(byte[] t)
        {
            var serializer = MessagePackSerializer.Create<T>();
            return serializer.UnpackSingleObject(t);
        }

        public static T Unpack<T>(this byte[] t)
        {
            var serializer = MessagePackSerializer.Create<T>();
            return serializer.UnpackSingleObject(t);
        }

        public static byte[] ToASCII(this string s)
        {
            return System.Text.Encoding.ASCII.GetBytes(s);
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
                            var res = my_plugin.call(
                            new call
                            {
                                name = "add".ToASCII(),
                                parameters = {
                                    new List<int>{2,3,5}.Pack()
                                }
                            }.Pack());
                            int call_res = Extensions.UnpackMsg<int>(res);
                            Console.WriteLine("Result: {0}", call_res);
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
