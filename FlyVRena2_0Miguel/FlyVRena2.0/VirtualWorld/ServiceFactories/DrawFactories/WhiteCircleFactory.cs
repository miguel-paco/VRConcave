using FlyVRena2._0.VirtualWorld.Services.DrawServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenCV.Net;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories.DrawFactories
{
    public class WhiteCircleFactory : ServiceFactory
    {
        [XmlElement("circleRadius")]
        public int circleRadius;
        [XmlElement("nPoints")]
        public int np;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            // Convert the radius from MillimeterCurve into VirtualRealityLinear (Approximation)
            External.Coordinates coord = new External.Coordinates() { MillimetersCurve = new Point2d(circleRadius, circleRadius) };
            External.Coordinates center = new External.Coordinates() { MillimetersCurve = new Point2d(0, 0) };
            int radius = Convert.ToInt32(((coord.VirtualRealityLine.X - center.VirtualRealityLine.X) + (coord.VirtualRealityLine.Y - center.VirtualRealityLine.Y)) / 2);
            //Console.WriteLine("{0}", radius);
            var draw = new WhiteCircleService(wo, VW, radius, np);
            wo.AddService(typeof(WhiteCircleService), draw);
        }
    }
}
