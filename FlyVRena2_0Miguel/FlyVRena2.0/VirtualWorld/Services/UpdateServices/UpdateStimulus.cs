using FlyVRena2._0.External;
using FlyVRena2._0.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCV.Net;
using FlyVRena2._0.VirtualWorld.Services.DrawServices;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateStimulus : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public float gain;
        public UpdateStimulus(IServiceContainer wObj, VirtualWorld VW, float gain) : base(wObj, VW)
        {
            posServ = new PositionService();
            this.gain = gain;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        public override void Update(double time)
        {

        }

        float[] currentStimSize = new float[2];
        double[] currentStimPosition = new double[2];

        protected override void Process(FilteredData data)
        {
            // Save Current Stimulus Position
            currentStimPosition[0] = positionService.position.X;
            currentStimPosition[1] = positionService.position.Y;
            currentStimSize[0] = 0f;
            currentStimSize[1] = 0f;
            this.Send<StimData>(new StimData(currentStimPosition, currentStimSize, data.clock, data.ID));

            // Update Stimulus Position (Feedback)
            Coordinates centroid = new Coordinates() { PixelsCurve = new Point2d(data.position[0], data.position[1]) };
            //Console.WriteLine("{0}", centroid.MillimetersLine);
            centroid.MillimetersCurve *= gain;
            this.positionService.position.X = Convert.ToSingle(centroid.VirtualRealityLine.X);
            this.positionService.position.Y = Convert.ToSingle(centroid.VirtualRealityLine.Y);
        }
    }
}
