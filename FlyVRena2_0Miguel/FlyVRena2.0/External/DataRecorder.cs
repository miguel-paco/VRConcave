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
            if (writeTracking)
            {
                if (cam != null)
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.clock.ToString() + " " + data.position[0].ToString() + " " + data.position[1].ToString() + " " + 
                        data.position[2].ToString() + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture) + " " + cam.m_s32FrameCoutTotal.ToString());
                }
                else
                {
                    fileStream.WriteLine(data.ID.ToString() + " " + data.clock.ToString() + " " +
                        data.position[0].ToString() + " " + data.position[1].ToString() + " " + data.position[2].ToString() + " " + (vw._time).ToString("F6", CultureInfo.InvariantCulture));
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
