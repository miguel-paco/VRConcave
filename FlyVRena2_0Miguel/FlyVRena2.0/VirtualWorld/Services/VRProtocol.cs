using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services
{
    public class VRProtocol
    {
        public string paramsPathCam1 = "";
        public string paramsPathCam2 = "";
        public bool useCam1 = false;
        public bool useCam2 = false;
        public bool trackCam1 = false;
        public bool trackCam2 = false;
        public bool dispCam1 = false;
        public bool dispCam2 = false;
        public bool recordCam1 = false;
        public bool recordCam2 = false;
        public int fpsCam1 = 1;
        public int fpsCam2 = 1;
        public string recordPathCam1 = "";
        public string recordPathCam2 = "";
        public bool usePulsePal = false;
        public string portPulsePal = "COM5";
        public bool recordTracking = false;
        public bool recordTrackingRaw = false;
        public string recordPathTracking = "";
        public float duration = 10;
        public bool recordStimulus = false;
        public string recordPathStimulus = "";

        public VRProtocol(IServiceProvider wObj, VirtualWorld VW, string cam1Params, string cam2Params, bool cam1Use, bool cam2Use, bool cam1Track, bool cam2Track,
            bool cam1Disp, bool cam2Disp, bool cam1Rec, bool cam2Rec, string cam1StringRec, string cam2StringRec, int cam1FPS, int cam2FPS,
            bool pulsePalUse, string pulsePalPort, bool trackingRec,
            string trackingRecPath, bool stimulusRec, string stimulusRecPath, float duration)
        {
            this.paramsPathCam1 = cam1Params;
            this.paramsPathCam2 = cam2Params;
            this.useCam1 = cam1Use;
            this.useCam2 = cam2Use;
            this.trackCam1 = cam1Track;
            this.trackCam2 = cam2Track;
            this.dispCam1 = cam1Disp;
            this.dispCam2 = cam2Disp;
            this.recordCam1 = cam1Rec;
            this.recordCam2 = cam2Rec;
            this.fpsCam1 = cam1FPS;
            this.fpsCam2 = cam2FPS;
            this.recordPathCam1 = cam1StringRec;
            this.recordPathCam2 = cam2StringRec;
            this.usePulsePal = pulsePalUse;
            this.portPulsePal = pulsePalPort;
            this.recordTracking = trackingRec;
            this.recordPathTracking = trackingRecPath;
            this.recordStimulus = stimulusRec;
            this.recordPathStimulus = stimulusRecPath;
            this.duration = duration;   
        }
    }
}
