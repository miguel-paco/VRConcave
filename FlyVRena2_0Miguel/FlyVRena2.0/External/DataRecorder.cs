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

namespace FlyVRena2._0.External
{
    public class DataRecorder<T> : Module<T> where T : FilteredData
    {
        // Vars Data Storage
        private StreamWriter fileStream;
        private string path;
        uEyeCamera cam;
        public Stopwatch watch;
        private bool writeTracking = false;
        private FlyVRena2._0.VirtualWorld.VirtualWorld vw;

        public DataRecorder(string path, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
        }

        public DataRecorder(string path, uEyeCamera camera, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            cam = camera;
        }

        public DataRecorder(string path, uEyeCamera camera, bool writeTracking, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            this.writeTracking = writeTracking;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            cam = camera;
        }

        public DataRecorder(string path, bool writeTracking, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            this.writeTracking = writeTracking;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
        }

        protected override void Process(T data)
        {
            Coordinates centroid = new Coordinates() { PixelsCurve = new Point2d(data.rawposition[0], data.rawposition[1]) };
            Coordinates head = new Coordinates() { PixelsCurve = new Point2d(data.head[0], data.head[1]) };
            float orientation = data.rawposition[2];

            // General save Structure:
            // FramesCam1 TrackingClock RawTrackingcentroidX(mm) RawTrackingCentroidY(mm) RawTrackingOrientation(deg) RawTrackingHeadX(mm) RawTrackingHeadY(mm) ExperimentTimer FramesCam2
            if (writeTracking)
            {
                if (cam != null)
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.clock.ToString() + " " + centroid.MillimetersCurve.X.ToString() + " " + centroid.MillimetersCurve.Y.ToString() + " " +
                        orientation.ToString() + " " + head.MillimetersCurve.X.ToString() + " " + head.MillimetersCurve.Y.ToString() + " " +
                        (vw._time).ToString("F6", CultureInfo.InvariantCulture) + " " + cam.m_s32FrameCoutTotal.ToString());
                }
                else
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.clock.ToString() + " " +
                        centroid.MillimetersCurve.X.ToString() + " " + centroid.MillimetersCurve.Y.ToString() + " " + orientation.ToString() + " " +
                        head.MillimetersCurve.X.ToString() + " " + head.MillimetersCurve.Y.ToString() + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture));
                }
            }
            else
            {
                if (cam != null)
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.clock.ToString() + " " +
                        data.raw[0].ToString() + " " + data.raw[1].ToString() + " " + data.raw[2].ToString() + " " + data.raw[3].ToString() + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture)
                       + " " + cam.m_s32FrameCoutTotal.ToString());
                }
                else
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.clock.ToString() + " " +
                        data.raw[0].ToString() + " " + data.raw[1].ToString() + " " + data.raw[2].ToString() + " " + data.raw[3].ToString() + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture));
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
