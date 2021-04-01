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
    public class UpdatePhotodiodeFactory : ServiceFactory
    {
        [XmlElement("frames")]
        public float frames;
   
        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var upd = new UpdatePhotodiode(wo, VW, frames);
            upd.Start();
            wo.AddService(typeof(UpdatePhotodiode), upd);
        }
    }
}
