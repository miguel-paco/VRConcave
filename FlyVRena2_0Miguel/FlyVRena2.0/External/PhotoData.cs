using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.External
{
    public class PhotoData : Message
    {
        public bool photobool;
        public float X;
        public string photoValue;

        public PhotoData(float X, bool photobool, string photoValue, ulong ID)
        {
            this.X = X;
            this.photobool = photobool;
            this.ID = ID;
            this.photoValue = photoValue;
        }
    }
}
