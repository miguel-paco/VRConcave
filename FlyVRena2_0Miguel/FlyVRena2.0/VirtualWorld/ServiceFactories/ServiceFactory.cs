using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories
{
    /* Objects that will initialize the services for the world objects */
    public abstract class ServiceFactory
    {
        public abstract void Initialize(IServiceProvider provider, VirtualWorld VW);
    }
}
