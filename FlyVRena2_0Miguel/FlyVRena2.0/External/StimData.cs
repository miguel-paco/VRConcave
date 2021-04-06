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
        public float state;

        public StimData(double[] position, float[] size, float state, double time, ulong ID)
        {
            this.position = position;
            this.size = size;
            this.state = state;
            this.time = time;
            this.ID = ID;
        }
    }
}