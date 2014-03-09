using MsgPack.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                            var res = SimpleExample(my_plugin);

                            // wrong call demo
                            res = TryWrongCalls(my_plugin, res);

                            // overhead measurement
                            int count = 10000000;
                            var numbers = Enumerable.Range(1, count).Reverse().ToList();
                            MeasureSortMax(my_plugin, count, numbers);
                            MeasureEcho(my_plugin, numbers);
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

        private static byte[] SimpleExample(zmq_plugin my_plugin)
        {
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
            return res;
        }

        private static byte[] TryWrongCalls(zmq_plugin my_plugin, byte[] res)
        {
            res = my_plugin.call(new call { name = "bla".ToASCII() }.Pack());
            string err_res = System.Text.Encoding.ASCII.GetString(Extensions.UnpackMsg<byte[]>(res));
            Console.WriteLine(err_res);
            res = my_plugin.call(new call { name = "add".ToASCII() }.Pack());
            err_res = System.Text.Encoding.ASCII.GetString(Extensions.UnpackMsg<byte[]>(res));
            Console.WriteLine(err_res);
            return res;
        }

        private static void MeasureSortMax(zmq_plugin my_plugin, int count, List<int> numbers)
        {
            Console.WriteLine("benchmarking communication [{0}]", count);
            Console.WriteLine("Message length: {0} MB",
            new call
            {
                name = "sort_max".ToASCII(),
                parameters = { numbers.Pack() }
            }.Pack().Length / 1048576f);
            var benchmark_call = new call
            {
                name = "sort_max".ToASCII(),
                parameters = {
                                                numbers.Pack()
                                            }
            }.Pack();
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(Extensions.Time(() =>
                {
                    var _ = my_plugin.call(benchmark_call);
                }));
            }
        }

        private static void MeasureEcho(zmq_plugin my_plugin, List<int> numbers)
        {

            // echo measurement
            Console.WriteLine("benchmarking echo+deserialization...");
            var echo_call = new call
            {
                name = "echo".ToASCII(),
                parameters = {
                                                numbers.Pack()
                                            }
            }.Pack();
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(Extensions.Time(() =>
                {
                    var _ = my_plugin.call(echo_call);
                }));
            }
        }
    }
}
