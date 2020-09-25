using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.ImageProcessing
{
    public class FilteredData : Message
    {
        public int[] raw;
        public float[] position;
        public float[] rawposition;
        public float[] head;
        public float[] velocity;
        public string source;
        public int clock;

        public FilteredData(ulong ID, string source, float[] position, float[] velocity, int[] raw, int clock)
        {
            this.ID = ID;
            this.source = source;
            this.position = position;
            this.velocity = velocity;
            this.raw = raw;
            this.clock = clock;
        }

        public FilteredData(ulong ID, string source, float[] position, float[] velocity, int[] raw, int clock, float[] rawposition, float[] head)
        {
            this.ID = ID;
            this.source = source;
            this.position = position;
            this.rawposition = rawposition;
            this.head = head;
            this.velocity = velocity;
            this.raw = raw;
            this.clock = clock;
        }
    }
}
