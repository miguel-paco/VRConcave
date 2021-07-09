﻿using FlyVRena2._0.External;
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
        public int protocol_numb;
        public float k = 0f; // Check if at least one FilteredData was generated
        public Coordinates centroid;


        // Stimulus variables
        float[] currentStimSize = new float[2];
        double[] currentStimPosition = new double[2];
        public float state = 0f;
        public double radius = 0f;
        double X;
        double Y;
        public double value;

        // Specific Variables
        // Generic Protocol Tools
        public double counter = 0.0;
        public float constant = 1000f;
        // Protocol 1 - Circular Motion
        public double vector_norm;
        public double walking_angle;
        // Protocol 3 - Sinusoid Aux
        public double L;
        public double circle_numb;
        public double dtheta;
        public double direction = 1;
        public double dx;
        




        public UpdateStimulus(IServiceContainer wObj, VirtualWorld VW, int protocol, float protocol_radius, float protocol_speed, int protocol_direction, float protocol_timer, int protocol_numb) : base(wObj, VW)
        {
            posServ = new PositionService();
            this.protocol = protocol;
            this.protocol_radius = protocol_radius;
            this.protocol_speed = protocol_speed;
            this.protocol_direction = protocol_direction;
            this.protocol_timer = protocol_timer;
            this.protocol_numb = protocol_numb;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        // Open Loop Section
        public override void Update(double time)
        {
            // Generate a Random Integer from 0 to 255
            var rand = new Random();
            var bytes = new byte[1];
            rand.NextBytes(bytes);
            int rnd = bytes[0];

            // Protocol 0: Follow Fly

            // Protocol 1: Circle -----------------------------------------------------
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

                X = Math.Cos(walking_angle) * protocol_radius;
                Y = Math.Sin(walking_angle) * protocol_radius;
            }

            // Protocol 2: Sudden Turn Circle ------------------------------------------
            if (protocol == 2)
            {
                if (k == 0)
                {
                    walking_angle = 0;
                }
                if (k == 1)
                {
                    if (state == 0)
                    {
                        if (counter == 0)
                        {
                            counter = Convert.ToSingle(rnd) * 10 + 5000; // 2500 to 5050 counts ~ 5 to 7.55 seconds
                        }
                        else
                        {
                            counter -= 1;
                        }
                        if (counter == 0)
                        {
                            state = 1;
                        }
                        else
                        {
                            vector_norm = protocol_speed * time;
                            walking_angle += (2 * Math.Asin((vector_norm / 2) / protocol_radius)) * protocol_direction;
                            radius = protocol_radius;
                        }
                    }
                    if (state == 1)
                    {
                        vector_norm = protocol_speed * time;
                        radius -= vector_norm;
                        if (radius < 0)
                        {
                            radius = -radius; ;
                            rnd = rnd - 255 / 2; // Generate a Random Number Between -127.5 (-128) and 127.5 (128)
                            walking_angle += (Convert.ToDouble(rnd) - 180) * Math.PI / 180;
                            state = 2;
                        }
                    }
                    else if (state == 2)
                    {
                        vector_norm = protocol_speed * time;
                        radius += vector_norm;
                        if (radius > protocol_radius)
                        {
                            vector_norm = radius - protocol_radius;
                            walking_angle += (2 * Math.Asin((vector_norm / 2) / protocol_radius)) * protocol_direction;
                            radius = protocol_radius;
                            state = 0;
                        }
                    }
                }

                X = Math.Cos(walking_angle) * radius;
                Y = Math.Sin(walking_angle) * radius;
            }

            // Protocol 3: Sinusoid (state = 0) / Ribbon (state = 1) / Circle (state = 2) -----------------------------------------------------
            if (protocol == 3)
            {
                if (radius == 0)
                {
                    walking_angle = 0;

                    L = protocol_radius * 2;
                    circle_numb = protocol_numb;

                    radius = L / (4 * circle_numb); // Diameter is L/(2 * the number of circle halfs) - radius is half the diameter
                    vector_norm = protocol_speed * time;
                    dtheta = 2 * Math.Asin(vector_norm / (2 * radius));
                    radius = vector_norm / (2 * Math.Sin(dtheta / 2));
                    L = radius * 4 * circle_numb;

                    Y = 0;
                    X = 0;
                    value = 1;

                    state = 2;
                }
                else if (k == 1)
                {
                    if (X >= L/2)
                    {
                        value = -1;
                    }
                    else if (X <= - L/2)
                    {
                        value = 1;
                    }
                    
                    vector_norm = protocol_speed * time;
                    dtheta = 2 * Math.Asin(vector_norm / (2 * radius));
                    walking_angle += dtheta;
                    dx = value * Math.Abs(radius * Math.Cos(walking_angle) - radius * Math.Cos(walking_angle - dtheta));
                    if (state != 0)
                    {
                        if ((X * (X + dx)) < 0)
                        {
                            if (state == 1)
                            {
                                direction = -direction;
                            }
                            if (state == 2)
                            {
                                value = -value;
                                dx = -dx;
                            }
                        }
                    }
                    X += dx;
                    Y = protocol_direction * direction * radius * Math.Sin(walking_angle);
                }
            }

            centroid = new Coordinates() { MillimetersCurve = new Point2d(X, Y) };

            this.positionService.position.X = Convert.ToSingle(centroid.VirtualRealityLine.X);
            this.positionService.position.Y = Convert.ToSingle(centroid.VirtualRealityLine.Y);
            this.positionService.rotation.Z = Convert.ToSingle(-walking_angle);

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

            // Save Stimulus
            currentStimPosition[0] = positionService.position.X;
            currentStimPosition[1] = positionService.position.Y;
            currentStimSize[0] = 0f;
            currentStimSize[1] = 0f;
            this.Send<StimData>(new StimData(currentStimPosition, currentStimSize, state, data.clock, data.ID));
        }
    }
}
