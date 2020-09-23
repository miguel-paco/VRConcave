using FlyVRena2._0.VirtualWorld.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories
{
    public class NameFactory : ServiceFactory
    {
        [XmlElement("name")]
        public string name;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var nameServ = new NameService(wo, VW, name);
            wo.AddService(typeof(NameService), nameServ);
        }
    }
}
