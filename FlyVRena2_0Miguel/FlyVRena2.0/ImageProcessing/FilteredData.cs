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
        private static readonly float[] vs = new float[5] { 0, 0, 0, 0, 0 };
        public float[] female = vs;
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

        public FilteredData(ulong ID, string source, float[] position, float[] velocity, float[] rawposition, float[] head, float[] femaleinfo, double clock, Photodiode pd)
        {
            this.ID = ID;
            this.source = source;
            this.position = position;
            this.velocity = velocity;
            this.rawposition = rawposition;
            this.head = head;
            this.clock = clock;
            this.photodiode = pd;
            this.female = femaleinfo;
        }
    }
}
