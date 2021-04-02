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
    public class PhotoRecorder<T> : Module<T> where T : PhotoData
    {
        // Vars Data Storage
        private StreamWriter fileStream;
        private string path;
        uEyeCamera cam;
        public Stopwatch watch;
        private bool writePhoto = false;
        private FlyVRena2._0.VirtualWorld.VirtualWorld vw;

        // Load Photodiode
        Photodiode photodiode;

        public PhotoRecorder(string path, Photodiode pd, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            photodiode = pd;
        }

        public PhotoRecorder(string path, Photodiode pd, uEyeCamera camera, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            cam = camera;
            photodiode = pd;
        }

        public PhotoRecorder(string path, Photodiode pd, uEyeCamera camera, bool writePhoto, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            this.writePhoto = writePhoto;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            cam = camera;
            photodiode = pd;
        }

        public PhotoRecorder(string path, Photodiode pd, bool writePhoto, FlyVRena2._0.VirtualWorld.VirtualWorld VW)
        {
            this.vw = VW;
            this.path = path;
            this.writePhoto = writePhoto;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
            photodiode = pd;
        }

        protected override void Process(T data)
        {

            //string photoLvl = photodiode.Read;
            ////// SEE PHOTODIODE RESULT
            ////if (photodiode != null)
            ////{
            ////    Console.WriteLine(photoLvl);
            ////}


            // General save Structure:
            // FramesCam1 TrackingClock PhotodiodeShadowState PhotodiodeValue
            // ExperimentTimer FramesCam2

            if (writePhoto)
            {
                if (cam != null)
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.X.ToString() + " " + data.photobool.ToString() + " " + data.photoValue + " " +
                        (vw._time).ToString("F6", CultureInfo.InvariantCulture) + " " + cam.m_s32FrameCoutTotal.ToString());
                }
                else
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.X.ToString() + " " + data.photobool.ToString() + " " + data.photoValue + " " +
                       (vw._time).ToString("F6", CultureInfo.InvariantCulture));
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
