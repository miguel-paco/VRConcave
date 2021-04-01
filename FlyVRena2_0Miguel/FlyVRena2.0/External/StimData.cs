using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.ImageProcessing
{
    public class StimData : Message
    {
        public double[] position;
        public float[] size;
        public double time;

        public StimData(double[] position, float[] size, double time, ulong ID)
        {
            this.position = position;
            this.size = size;
            this.time = time;
            this.ID = ID;
        }
    }
}