using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zmqpluginuser
{
    public enum load_result
    {
        OK =0,
        FAILED_LOADING,
        FAILED_CONNECTING
    }

    public class zmq_plugin : IDisposable
    {
        public delegate int plugin_loader_delegate([MarshalAs(UnmanagedType.LPStr)] string config);
        UnmanagedLibrary my_plugin;
        plugin_loader_delegate plugin_loader;
        ZMQ.Context context;
        ZMQ.Socket client;

        public load_result load(string config)
        {
            int res = plugin_loader(config);
            if (res != 0)
                return load_result.FAILED_LOADING;

            var cfg = JsonConvert.DeserializeObject<config>(config);
            context = new ZMQ.Context();
            client = context.Socket(ZMQ.SocketType.PAIR);
            try
            {
                client.Connect(cfg.connect_string);
            }
            catch
            {
                return load_result.FAILED_CONNECTING;
            }
            return load_result.OK;
        }

        public zmq_plugin(string zmq_plugin_dll)
        {
            my_plugin = new UnmanagedLibrary(zmq_plugin_dll);
            plugin_loader = my_plugin.GetUnmanagedFunction<plugin_loader_delegate>("load_plugin");
        }

        public void Dispose()
        {
            if (client != null)
                client.Dispose();
            if (context != null)
                context.Dispose();
            if (my_plugin != null)
                my_plugin.Dispose();
        }

        public string hello()
        {
            client.Send("hello", System.Text.Encoding.ASCII);
            return client.Recv(System.Text.Encoding.ASCII);
        }

        public byte[] call(byte[] message)
        {
            client.Send(message);
            return client.Recv();
        }
    }
}
