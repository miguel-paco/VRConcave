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
        public float protocol_timer;
        public float k = 0f; // Check if at least one FilteredData was generated
        public Coordinates centroid;


        // Stimulus variables
        float[] currentStimSize = new float[2];
        double[] currentStimPosition = new double[2];

        // Specific Variables
        // Generic Protocol Tools
        public float counter = 0f;
        public float constant = 1000f;
        // Protocol 1 - Circular Motion
        public double vector_norm;
        public double walking_angle;
        // Protocol 100 - Flicker
        public float photodiodeShadowTimer = 100; // milliseconds
        public bool photodiodeSwitch = false;



        public UpdateStimulus(IServiceContainer wObj, VirtualWorld VW, int protocol, float protocol_radius, float protocol_speed, int protocol_direction, float protocol_timer) : base(wObj, VW)
        {
            posServ = new PositionService();
            this.protocol = protocol;
            this.protocol_radius = protocol_radius;
            this.protocol_speed = protocol_speed;
            this.protocol_direction = protocol_direction;
            this.protocol_timer = protocol_timer;
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
                this.positionService.rotation.Z = Convert.ToSingle(-walking_angle);

            }

            // Protocol 100: Photodiode Flicker
            if (protocol == 100)
            {
                counter += 1;

                if (counter == protocol_timer)
                {
                    counter = 0;
                    constant = -constant;
                    this.positionService.position.X += constant;
                    this.positionService.position.Y += constant;
                    photodiodeSwitch = !photodiodeSwitch;
                }
            }
            
        }


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
                this.positionService.rotation.Z = Convert.ToSingle(Math.PI * data.position[2] / 180);
            }

            // Save Stimulus Position
            if (protocol != 100)
            {   
               currentStimPosition[0] = positionService.position.X;
               currentStimPosition[1] = positionService.position.Y;
               currentStimSize[0] = 0f;
               currentStimSize[1] = 0f;
               this.Send<StimData>(new StimData(currentStimPosition, currentStimSize, photodiodeSwitch, data.clock, data.ID));
            }
        }
    }
}
