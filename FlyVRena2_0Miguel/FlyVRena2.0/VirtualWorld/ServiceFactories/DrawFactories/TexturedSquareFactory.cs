using FlyVRena2._0.VirtualWorld.Services.DrawServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace FlyVRena2._0.VirtualWorld.ServiceFactories.DrawFactories
{
    public class TexturedSquareFactory : ServiceFactory
    {
        [XmlElement("texturePath")]
        public string texturePath;
        [XmlElement("textureSize")]
        public int textureSize;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var draw = new TexturedSquareService(wo, VW, texturePath, textureSize);
            wo.AddService(typeof(TexturedSquareService), draw);
        }
    }
}
