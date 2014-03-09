using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zmqpluginuser
{
    public class config
    {
        public string dll { get; set; }
        public string protocol { get; set; }
        public string bind_string { get; set; }
        public string connect_string { get; set; }
    }
}
