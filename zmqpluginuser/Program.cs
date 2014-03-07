using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zmqpluginuser
{
    class Program {

        delegate int plugin_loader([MarshalAs(UnmanagedType.LPStr)] string config);

        static void Main(string[] args)
        {
            try
            {
                using (UnmanagedLibrary my_plugin = new UnmanagedLibrary("zmqplugin.dll"))
                {
                    var load_plugin = my_plugin.GetUnmanagedFunction<plugin_loader>("load_plugin");
                    if (load_plugin != null)
                        Console.WriteLine(load_plugin("bla"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
