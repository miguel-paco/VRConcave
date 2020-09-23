using FlyVRena2._0.VirtualWorld.Services;
using FlyVRena2._0.VirtualWorld.Services.UpdateServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories.UpdateFactories
{
    public class RFMappingRotDotStimFactory : ServiceFactory
    {
        [XmlElement("amplitude")]
        public float amp;
        [XmlElement("size")]
        public float size;
        [XmlElement("speed")]
        public float speed;
        [XmlElement("time")]
        public float tTime;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var upd = new RFMappingRotDotStim(wo, VW, amp, size, speed, tTime);
            upd.Start();
            wo.AddService(typeof(RFMappingRotDotStim), upd);
        }
    }
}
