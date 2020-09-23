using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services
{
    /* Service that gives the World Object name */
    public class NameService
    {
        public string name;
        public NameService(IServiceProvider wObj, VirtualWorld VW, string str)
        {
            name = str;
        }
        /* Method that returns the name */
        public string ObjectName()
        {
            return name;
        }
    }
}
