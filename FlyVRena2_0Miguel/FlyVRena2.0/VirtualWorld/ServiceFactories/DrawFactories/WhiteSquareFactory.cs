using FlyVRena2._0.VirtualWorld.Services.DrawServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories.DrawFactories
{
    public class WhiteSquareFactory : ServiceFactory
    {
        [XmlElement("squareSize")]
        public int squareSize;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var draw = new WhiteSquareService(wo, VW, squareSize);
            wo.AddService(typeof(WhiteSquareService), draw);
        }
    }
}
