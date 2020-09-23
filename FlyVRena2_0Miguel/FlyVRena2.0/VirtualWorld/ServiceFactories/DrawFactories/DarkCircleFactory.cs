using FlyVRena2._0.VirtualWorld.Services.DrawServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace FlyVRena2._0.VirtualWorld.ServiceFactories.DrawFactories
{
    public class DarkCircleFactory : ServiceFactory
    {
        [XmlElement("circleRadius")]
        public int circleRadius;
        [XmlElement("nPoints")]
        public int np;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var draw = new DarkCircleService(wo, VW, circleRadius, np);
            wo.AddService(typeof(DarkCircleService), draw);
        }
    }
}
