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
        static void Main(string[] args)
        {
            try
            {
                using (var my_plugin = new zmq_plugin("zmqplugin.dll"))
                {
                    Console.WriteLine(my_plugin.load("bla"));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
