using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zmqpluginuser
{
    public class call
    {
        public byte[] name { get; set; }

        List<byte[]> parameters_ = new List<byte[]>();
        public IList<byte[]> parameters {
            get {
                return parameters_;
            }
        }
    }
}
