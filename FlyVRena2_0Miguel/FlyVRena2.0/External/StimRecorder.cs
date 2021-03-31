using FlyVRena2._0.ImageProcessing;
using Sardine.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCV.Net;
using System.Text.RegularExpressions;

namespace FlyVRena2._0.External
{
    public class StimRecorder<T> : Module<T> where T : StimData
    {
        // Vars Data Storage
        private StreamWriter fileStream;
        private string path;
        uEyeCamera cam;
        public Stopwatch watch;
        private bool writeStimulus = false;
        private FlyVRena2._0.VirtualWorld.VirtualWorld vw;
        private bool photobool = false;

        Photodiode lightValue;

        public StimRecorder(string path, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
        }

        public StimRecorder(string path, uEyeCamera camera, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            cam = camera;
        }

        public StimRecorder(string path, uEyeCamera camera, bool writeStimulus, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            this.writeStimulus = writeStimulus;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            cam = camera;
        }

        public StimRecorder(string path, bool writeStimulus, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            this.writeStimulus = writeStimulus;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
        }

        public StimRecorder(string path, Photodiode pd, uEyeCamera camera, bool writeStimulus, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            this.writeStimulus = writeStimulus;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            cam = camera;
            lightValue = pd;
        }

        public StimRecorder(string path, Photodiode pd, bool writeStimulus, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {

            this.vw = VW;
            this.path = path;
            this.writeStimulus = writeStimulus;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            lightValue = pd;
        }

        protected override void Process(T data)
        {
            Coordinates center = new Coordinates() { VirtualRealityLine = new Point2d(data.position[0], data.position[1]) };
            float[] size = data.size;
            string photoLvl = lightValue.Read;

            // SEE PHOTODIODE RESULT
            //if (lightValue != null)
            //{
            //    Console.WriteLine(photoLvl);
            //}

            // General save Structure:
            // FramesCam1 TrackingClock StimulusCenterPositionX(mm) StimulusCenterPositionY(mm)
            // StimulusSize1(mm) StimulusSize2(mm) PhotodiodeValue ExperimentTimer FramesCam2

            if (writeStimulus)
            {
                if (cam != null)
                {
                    if (lightValue != null)
                    {
                        fileStream.WriteLine(data.ID.ToString() + " " + data.time.ToString() + " " + center.MillimetersCurve.X.ToString() + " " + center.MillimetersCurve.Y.ToString() + " " +
                            size[0].ToString() + " " + size[1].ToString() + " " + photobool.ToString() + " " + photoLvl + " " +
                            (vw._time).ToString("F6", CultureInfo.InvariantCulture) + " " + cam.m_s32FrameCoutTotal.ToString());
                    }
                    else
                    {
                        fileStream.WriteLine(data.ID.ToString() + " " + data.time.ToString() + " " + center.MillimetersCurve.X.ToString() + " " + center.MillimetersCurve.Y.ToString() + " " +
                            size[0].ToString() + " " + size[1].ToString() + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture) + " " + cam.m_s32FrameCoutTotal.ToString());
                    }
                }
                else
                {
                    if (lightValue != null)
                    {
                        fileStream.WriteLine(data.ID.ToString() + " " + data.time.ToString() + " " + center.MillimetersCurve.X.ToString() + " " + center.MillimetersCurve.Y.ToString() + " " +
                        size[0].ToString() + " " + size[1].ToString() + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        fileStream.WriteLine(data.ID.ToString() + " " + data.time.ToString() + " " + center.MillimetersCurve.X.ToString() + " " + center.MillimetersCurve.Y.ToString() + " " +
                        size[0].ToString() + " " + size[1].ToString() + " " + photobool.ToString() + " " + photoLvl + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        public void OnExit()
        {
            fileStream.Close();
            fileStream.Dispose();
            this.Abort();
        }

        public void DisposeModule()
        {
            this.Dispose();
        }

    }
}
