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
        // position[0] = center X coordinate
        // position[1] = center Y coordinate
        // position[2] = orientation
        // velocity[0] = forward velocity
        // velocity[1] = sideways velocity
        // velocity[2] = angular velocity

        public float[] position = new float[3];
        public float[] head = new float[2]; // fly head position
        public float[] velocity = new float[3];
        public double clock;
        public string source;
        public MovementData(ulong ID, string source, float[] vals, float[] headcoord, double time)
        {
            this.ID = ID;
            this.source = source;
            this.position[0] = vals[0];
            this.position[1] = vals[1];
            this.position[2] = vals[2];
            this.velocity[0] = vals[3];
            this.velocity[1] = vals[4];
            this.velocity[2] = vals[5];
            this.head[0] = headcoord[0];
            this.head[1] = headcoord[1];
            this.clock = time;
        }
    }
}
