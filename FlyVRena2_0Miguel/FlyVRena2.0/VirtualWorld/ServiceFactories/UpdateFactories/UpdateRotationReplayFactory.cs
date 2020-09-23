﻿using FlyVRena2._0.VirtualWorld.Services;
using FlyVRena2._0.VirtualWorld.Services.UpdateServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories.UpdateFactories
{
    public class UpdateRotationReplayFactory : ServiceFactory
    {
        [XmlElement("gain")]
        public float gain;
        [XmlArray("Trials")]
        public List<Trial> trials;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var upd = new UpdateRotationReplay(wo, VW, gain, trials);
            upd.Start();
            wo.AddService(typeof(UpdateRotationReplay), upd);
        }
    }
}
