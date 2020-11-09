using FlyVRena2._0.External;
using FlyVRena2._0.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCV.Net;
using FlyVRena2._0.VirtualWorld.Services.DrawServices;
using uEye.Defines.IO;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateStimulus : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public int protocol;
        public float protocol_radius;
        public float protocol_speed;
        public int protocol_direction;
        public float k = 0f; // Check if at least one FilteredData was generated
        public Coordinates centroid;

        // Specific Variables
        // Protocol 1
        public double vector_norm;
        public double walking_angle;

        public UpdateStimulus(IServiceContainer wObj, VirtualWorld VW, int protocol, float protocol_radius, float protocol_speed, int protocol_direction) : base(wObj, VW)
        {
            posServ = new PositionService();
            this.protocol = protocol;
            this.protocol_radius = protocol_radius;
            this.protocol_speed = protocol_speed;
            this.protocol_direction = protocol_direction;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        // Open Loop Section
        public override void Update(double time)
        {
            // Protocol 0: Follow Fly

            // Protocol 1: Circle
            if (protocol == 1)
            {
                if (k == 0)
                {
                    walking_angle = 0;
                }
                
                if (k == 1)
                {
                    vector_norm = protocol_speed * time;
                    walking_angle += (2 * Math.Asin((vector_norm / 2) / protocol_radius)) * protocol_direction;
                }

                centroid = new Coordinates() { MillimetersCurve = new Point2d(Math.Cos(walking_angle) * protocol_radius, Math.Sin(walking_angle) * protocol_radius) };

                this.positionService.position.X = Convert.ToSingle(centroid.VirtualRealityLine.X);
                this.positionService.position.Y = Convert.ToSingle(centroid.VirtualRealityLine.Y);
            }

        }

        float[] currentStimSize = new float[2];
        double[] currentStimPosition = new double[2];


        protected override void Process(FilteredData data)
        {
            if (k == 0)
            {
                k = 1;
            }

            // Close Loop Section

            // Protocol 0: Follow Fly
            if (protocol == 0)
            {
                centroid = new Coordinates() { PixelsCurve = new Point2d(data.position[0], data.position[1]) };
                //Console.WriteLine("{0}", centroid.MillimetersLine);
                this.positionService.position.X = Convert.ToSingle(centroid.VirtualRealityLine.X);
                this.positionService.position.Y = Convert.ToSingle(centroid.VirtualRealityLine.Y);
            }

            // Save Stimulus Position
            currentStimPosition[0] = positionService.position.X;
            currentStimPosition[1] = positionService.position.Y;
            currentStimSize[0] = 0f;
            currentStimSize[1] = 0f;
            this.Send<StimData>(new StimData(currentStimPosition, currentStimSize, data.clock, data.ID));
        }
    }
}
