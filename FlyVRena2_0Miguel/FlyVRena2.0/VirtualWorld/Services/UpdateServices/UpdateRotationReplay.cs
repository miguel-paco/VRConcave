using FlyVRena2._0.ImageProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateRotationReplay : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public List<Trial> trials;
        public float gain;
        StreamReader sr;
        private float[] calibValues;
        private readonly float FR = 4000;

        public UpdateRotationReplay(IServiceContainer wObj, VirtualWorld VW, float gain, List<Trial> trials) : base(wObj, VW)
        {
            calibValues = new float[2];
            calibValues[0] = -0.0257f; //mm/tick
            calibValues[1] = (float)Math.PI * 0.449f / 180.0f; //º/tick;
            posServ = new PositionService();
            this.trials = trials;
            this.gain = gain;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
            sr = new StreamReader(GetPath());
        }

        double auxTime = 0;
        int i = 0;
        double timeT = 0;
        public override void Update(double time)
        {
            timeT += time;
            if (this.nameService.name == trials.ElementAt(i).TrialType)
            {
                this.positionService.position.Z = 1f;
                this.positionService.position.X = 0f;
                this.positionService.position.Y = 0f;

            }
            else
                this.positionService.position.Z = -1;

            if (timeT - auxTime >= trials.ElementAt(i).TrialDuration)
            {
                auxTime = timeT;
                if (i < trials.Count - 1)
                    i += 1;
            }
        }

        public string GetPath()
        {
            string path = " ";
            Thread t = new Thread((ThreadStart)(() =>
            {
                OpenFileDialog pathBrowserDialog = new System.Windows.Forms.OpenFileDialog();
                pathBrowserDialog.InitialDirectory = "c:\\";
                pathBrowserDialog.Filter = "txt files (*.txt)|*.txt";
                pathBrowserDialog.FilterIndex = 2;
                pathBrowserDialog.RestoreDirectory = true;
                if (pathBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    path = pathBrowserDialog.FileName; 
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            return path;
        }

        int k = 1;
        protected override void Process(FilteredData data)
        {
            if (k == 1)
            {
                k = 0;
                timeT = 0;
            }
            float[] vels = Calibrate(GetVels(ReadParseString()));
            this.positionService.rotation.Z += vels[2];
        }

        public int[] ReadParseString()
        {
            int[] vals = new int[4];
            if(sr.Peek() >= 0)
            {
                string str = sr.ReadLine();
                string[] nums = str.Split(' ');
                vals[0] = Convert.ToInt32(nums[2]);
                vals[1] = Convert.ToInt32(nums[3]);
                vals[2] = Convert.ToInt32(nums[4]);
                vals[3] = Convert.ToInt32(nums[5]);
            }
            return vals;
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
    }
}
