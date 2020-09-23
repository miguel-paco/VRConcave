using FlyVRena2._0.External;
using Sardine.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.ImageProcessing
{
    public class TransformTreadmill<T> : Module<T> where T : TreadmillData
    {
        private float[] prevVals = new float[3];
        private float[] calibValues;
        private readonly float FR = 4000;

        public TransformTreadmill()
        {
            calibValues = new float[2];
            calibValues[0] = -0.0257f; //mm/tick
            calibValues[1] = (float)Math.PI * 0.449f/ 180.0f; //º/tick;
        }

        private float[] GetVels(int[] vals)
        {
            float[] vels = new float[3];
            vels[0] = -0.5f * (float)Math.Sqrt(2) * (vals[1] + vals[3]);
            vels[1] = -0.5f * (float)Math.Sqrt(2) * (vals[1] - vals[3]);
            vels[2] = -0.5f * (vals[0] + vals[2]);
            return vels;
        }

        private float[] Calibrate(float[] data)
        {
            float[] cdata = new float[3];
            cdata[0] = calibValues[0] * data[0];
            cdata[1] = calibValues[0] * data[1];
            cdata[2] = calibValues[1] * data[2];
            return cdata;
        }

        float[] pos = new float[3];
        protected override void Process(T data)
        {
            float[] vels = Calibrate(GetVels(data.values));
            pos[2] += vels[2] * FR;
            pos[0] += (vels[1] * (float)Math.Cos(Math.PI * pos[2] / 180) + vels[0] * (float)Math.Sin(Math.PI * pos[2] / 180)) * FR;
            pos[1] += (-vels[1] * (float)Math.Sin(Math.PI * pos[2] / 180) + vels[0] * (float)Math.Cos(Math.PI * pos[2] / 180)) * FR;
            Send<MovementData>(new MovementData(data.ID, data.source, new float[] {pos[0], pos[1], pos[2], vels[0], vels[1], vels[2] }, data.values, data.clock));
        }

        public void OnExit()
        {
            this.Abort();
        }

        public void DisposeModule()
        {
            this.Dispose();
        }
    }
}
