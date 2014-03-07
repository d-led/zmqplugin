using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zmqpluginuser
{
    public class zmq_plugin : IDisposable
    {
        public delegate int plugin_loader_delegate([MarshalAs(UnmanagedType.LPStr)] string config);
        UnmanagedLibrary my_plugin;
        plugin_loader_delegate plugin_loader;
        
        public int load(string config) {
            return plugin_loader(config);
        }

        public zmq_plugin(string zmq_plugin_dll)
        {
            my_plugin = new UnmanagedLibrary(zmq_plugin_dll);
            plugin_loader = my_plugin.GetUnmanagedFunction<plugin_loader_delegate>("load_plugin");
        }

        public void Dispose()
        {
            if (my_plugin != null)
                my_plugin.Dispose();
        }
    }
}
