using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyVRena2._0.External;

namespace FlyVRena2._0.ImageProcessing
{
    public class FilteredData : Message
    {
        public float[] position;
        public float[] rawposition;
        public float[] head;
        public float[] velocity;
        public string source;
        public double clock;
        public Photodiode photodiode;

        public FilteredData(ulong ID, string source, float[] position, float[] velocity, float[] rawposition, float[] head, double clock, Photodiode pd)
        {
            this.ID = ID;
            this.source = source;
            this.position = position;
            this.velocity = velocity;
            this.rawposition = rawposition;
            this.head = head;
            this.clock = clock;
            this.photodiode = pd;
        }
    }
}
