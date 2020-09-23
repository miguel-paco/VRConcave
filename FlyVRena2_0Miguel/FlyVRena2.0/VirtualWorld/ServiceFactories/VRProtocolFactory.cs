using FlyVRena2._0.VirtualWorld.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlyVRena2._0.VirtualWorld.ServiceFactories
{
    public class VRProtocolFactory : ServiceFactory
    {
        [XmlElement("UseCam1")]
        public bool cam1Use;
        [XmlElement("UseCam2")]
        public bool cam2Use;

        [XmlElement("PathParamsCam1")]
        public string cam1pathParams;
        [XmlElement("PathParamsCam2")]
        public string cam2pathParams;

        [XmlElement("Cam1Disp")]
        public bool cam1Disp;
        [XmlElement("Cam2Disp")]
        public bool cam2Disp;

        [XmlElement("Cam1Track")]
        public bool cam1Track;
        [XmlElement("Cam2Track")]
        public bool cam2Track;

        [XmlElement("RecordCam1")]
        public bool cam1Rec;
        [XmlElement("RecordCam2")]
        public bool cam2Rec;

        [XmlElement("PathRecordCam1")]
        public string cam1pathRec;
        [XmlElement("PathRecordCam2")]
        public string cam2pathRec;

        [XmlElement("FPSCam1")]
        public int cam1FPS;
        [XmlElement("FPSCam2")]
        public int cam2FPS;

        [XmlElement("UsePulsePal")]
        public bool usePulsePal;
        [XmlElement("PortPulsePal")]
        public string ppPort;

        [XmlElement("RecordTracking")]
        public bool recordTracking;
        [XmlElement("RecordTrackingRaw")]
        public bool recordTrackingRaw;
        [XmlElement("PathRecordingTracking")]
        public string pathRecordingTracking;

        [XmlElement("DurationInSeconds")]
        public float duration;

        public override void Initialize(IServiceProvider provider, VirtualWorld VW)
        {
            var wo = (IServiceContainer)provider.GetService(typeof(IServiceContainer));
            var vrProtocol = new VRProtocol(wo, VW, cam1pathParams, cam2pathParams, cam1Use, cam2Use, cam1Track, cam2Track, cam1Disp, cam2Disp, cam1Rec,
                cam2Rec, cam1pathRec, cam2pathRec, cam1FPS, cam2FPS, usePulsePal,
                ppPort, recordTracking, recordTrackingRaw, pathRecordingTracking, duration);
            wo.AddService(typeof(VRProtocol), vrProtocol);
        }
    }
}
