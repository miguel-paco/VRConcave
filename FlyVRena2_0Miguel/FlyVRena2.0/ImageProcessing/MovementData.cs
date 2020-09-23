using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.ImageProcessing
{
    public class MovementData : Message
    {
        public string source;
        public int[] raw = new int[4];
        public float[] position = new float[3];
        public float[] head = new float[2]; // fly head position
        public float[] velocity = new float[3];
        public int clock = 0;
        public MovementData(ulong ID, string Source, float[] vals)
        {
            this.ID = ID;
            this.source = Source;
            this.position[0] = vals[0];
            this.position[1] = vals[1];
            this.position[2] = vals[2];
            this.velocity[0] = vals[3];
            this.velocity[1] = vals[4];
            this.velocity[2] = vals[5];
        }

        public MovementData(ulong ID, string Source, float[] vals, int[] raw, int clock)
        {
            this.ID = ID;
            this.source = Source;
            this.position[0] = vals[0];
            this.position[1] = vals[1];
            this.position[2] = vals[2];
            this.velocity[0] = vals[3];
            this.velocity[1] = vals[4];
            this.velocity[2] = vals[5];
            this.clock = clock;
            this.raw = raw;
        }

        public MovementData(ulong ID, string Source, float[] vals, float[] headcoord)
        {
            this.ID = ID;
            this.source = Source;
            this.position[0] = vals[0];
            this.position[1] = vals[1];
            this.position[2] = vals[2];
            this.velocity[0] = vals[3];
            this.velocity[1] = vals[4];
            this.velocity[2] = vals[5];
            this.head[0] = headcoord[0];
            this.head[1] = headcoord[1];
        }

        public MovementData(ulong ID, string Source, float[] vals, int[] raw, int clock, float[] headcoord)
        {
            this.ID = ID;
            this.source = Source;
            this.position[0] = vals[0];
            this.position[1] = vals[1];
            this.position[2] = vals[2];
            this.velocity[0] = vals[3];
            this.velocity[1] = vals[4];
            this.velocity[2] = vals[5];
            this.clock = clock;
            this.raw = raw;
            this.head[0] = headcoord[0];
            this.head[1] = headcoord[1];
        }
    }
}
