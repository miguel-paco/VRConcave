﻿using FlyVRena2._0.VirtualWorld.Services;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenCV.Net;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories
{
    public class PositionFactory : ServiceFactory
    {
        [XmlElement("x")]
        public float x;
        [XmlElement("y")]
        public float y;
        [XmlElement("z")]
        public float z;
        [XmlElement("rx")]
        public float rx;
        [XmlElement("ry")]
        public float ry;
        [XmlElement("rz")]
        public float rz;
        [XmlElement("scale")]
        public float scale;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            External.Coordinates coord = new External.Coordinates() { MillimetersCurve = new Point2d(x, y) };
            var pos = new PositionService(wo, VW, new Vector3(Convert.ToSingle(coord.VirtualRealityLine.X), Convert.ToSingle(coord.VirtualRealityLine.Y), z) , new Vector3(rx, ry, rz), scale);
            wo.AddService(typeof(PositionService), pos);
        }

    }
}
