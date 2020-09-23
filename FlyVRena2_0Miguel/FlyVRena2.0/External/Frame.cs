using OpenCV.Net;
using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.External
{
    //Frame object is a disposable object containing an image and a number
    public class Frame : Message, IDisposable
    {
        public IplImage image;
        public long frameNo;
        public long frameRate;
        public string source = "";

        public Frame(IplImage image, long frameNo)
        {
            this.frameNo = frameNo;
            this.image = image;
        }

        public Frame(IplImage image, long frameNo, long frameRate, string status)
        {
            this.ID = (ulong)frameNo;
            this.frameNo = frameNo;
            this.image = image;
            this.frameRate = frameRate;
            this.source = "Camera"  + status;
        }

        public Frame()
        {
            frameNo= 0;
        }

        public Frame Copy()
        {
            return new Frame(this.image.Clone(), this.frameNo);
        }

        public void Dispose()
        {
            image.Dispose();
        }
    }
}
