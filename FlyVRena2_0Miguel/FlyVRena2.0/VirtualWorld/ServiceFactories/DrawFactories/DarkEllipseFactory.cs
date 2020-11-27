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
    public class DarkEllipseFactory : ServiceFactory
    {
        [XmlElement("ellipseXRadius")]
        public double ellipseXRadius;
        [XmlElement("ellipseYRadius")]
        public double ellipseYRadius;
        [XmlElement("nPoints")]
        public int np;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            // Convert the radius from MillimeterCurve into VirtualRealityLinear (Approximation)
            External.Coordinates coordX = new External.Coordinates() { MillimetersCurve = new Point2d(ellipseXRadius, ellipseXRadius) };
            External.Coordinates coordY = new External.Coordinates() { MillimetersCurve = new Point2d(ellipseYRadius, ellipseYRadius) };
            External.Coordinates center = new External.Coordinates() { MillimetersCurve = new Point2d(0, 0) };
            int radiusX = Convert.ToInt32(((coordX.VirtualRealityLine.X - center.VirtualRealityLine.X) + (coordX.VirtualRealityLine.Y - center.VirtualRealityLine.Y)) / 2);
            int radiusY = Convert.ToInt32(((coordY.VirtualRealityLine.X - center.VirtualRealityLine.X) + (coordY.VirtualRealityLine.Y - center.VirtualRealityLine.Y)) / 2);
            //Console.WriteLine("{0}", radius);
            var draw = new DarkEllipseService(wo, VW, radiusX, radiusY, np);
            wo.AddService(typeof(DarkEllipseService), draw);
        }
    }
}
