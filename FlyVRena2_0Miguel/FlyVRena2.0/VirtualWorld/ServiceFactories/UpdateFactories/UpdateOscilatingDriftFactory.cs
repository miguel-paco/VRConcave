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
    public class UpdateOscilatingDriftFactory : ServiceFactory
    {
        [XmlElement("rSpeed")]
        public float rSpeed;
        [XmlElement("amplitude")]
        public float amplitude;
        [XmlElement("direction")]
        public string direction;
        [XmlArray("Trials")]
        public List<Trial> trials;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var upd = new UpdateOscilatingDrift(wo, VW, rSpeed, amplitude, direction, trials);
            upd.Start();
            wo.AddService(typeof(UpdateOscilatingDrift), upd);
        }
    }
}
