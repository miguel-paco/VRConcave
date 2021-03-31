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
    public class UpdateStimulusFactory : ServiceFactory
    {

        [XmlElement("protocol")]
        public int protocol;

        [XmlElement("protocol_radius")]
        public float protocol_radius;

        [XmlElement("protocol_speed")]
        public float protocol_speed;

        [XmlElement("protocol_direction")]
        public int protocol_direction;

        [XmlElement("protocol_timer_ms")]
        public float protocol_timer;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var upd = new UpdateStimulus(wo, VW, protocol, protocol_radius, protocol_speed, protocol_direction, protocol_timer);
            upd.Start();
            wo.AddService(typeof(UpdateStimulus), upd);
        }

    }
}
