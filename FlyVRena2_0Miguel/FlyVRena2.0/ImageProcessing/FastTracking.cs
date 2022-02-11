using FlyVRena2._0.External;
using FlyVRena2._0.Visualizers;
using OpenCV.Net;
using System.Collections.Generic;
using System.Linq;
using Sardine.Core;
using System;
using System.Diagnostics;
using System.Globalization;



namespace FlyVRena2._0.ImageProcessing
{
    public class FastTracking<T> : Module<T> where T : Frame
    {
        Stopwatch stopWatch = new Stopwatch(); // Check process time auxiliar variable

        bool boolDisplayTrackingResult;
        List<float[]> trackPreviousResult = new List<float[]>();
        private IplImage Background;
        private float[] trackMovementDirection = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private FlyVRena2._0.VirtualWorld.VirtualWorld vw;

        public int nFlies = 1;
        private int _minArea = 0; 
        public int MinArea
        {
            get => _minArea;
            set
            {
                _minArea = value;
                Changed("FlyMinArea");
            }
        }
        private int _maxArea = 0; 
        public int MaxArea
        {
            get => _maxArea;
            set
            {
                _maxArea = value;
                Changed("FlyMaxArea");
            }
        }
        private int _thr = 0; 
        public int Thr
        {
            get => _thr;
            set
            {
                _thr = value;
                Changed("FlyThreshold");
            }
        }
        private int _smooth = 1;
        public int SmoothSize
        {
            get => _smooth;
            set
            {
                _smooth = value;
                Changed("FlySmoothSize");
            }
        }
        
        //Tracking Visualizer Parameters
        TypeVisualizerDialog vis;
        ImageVisualizer imVis;
        ServiceProvider provider;


        public FastTracking(FlyVRena2._0.VirtualWorld.VirtualWorld VW, Frame mask, int minA, int maxA, int thr, int smooth, bool twoflies, bool boolDisplayTrackingResult)
        {
            this.vw = VW;
            //Initialize with pre-defined parameters. Modify and re-compile to alter tracking parameters. 
            MinArea = minA;
            MaxArea = maxA;
            Thr = thr;
            SmoothSize = smooth;
            if (twoflies)
            {
                nFlies = 2;
            }
            this.boolDisplayTrackingResult = boolDisplayTrackingResult;
            Background = mask.image; // In background subtraction method, this will be the first adquired frame 
            if (boolDisplayTrackingResult)
            {
                provider = new ServiceProvider();
                vis = new TypeVisualizerDialog();
                provider.services.Add(vis);
                imVis = new ImageVisualizer();
                imVis.Load(provider, 512, 512);
                vis.Show();
                vis.Location = new System.Drawing.Point(512, 0);
            }
        }
        
        public List<float[]> ObtainTrackingData(IplImage output)
        {
            List<float[]> flyParams = new List<float[]>();
            
            if (Background == null) // if no first frame
            {
                Background = output.Clone();
            }
            else
            {
                List<float[]> paramOutput = new List<float[]>();
                List<double> bfArea = new List<double>();

                double contourArea;
                RotatedRect elipse;
                Moments moments;

                output = BackgroundSubtraction(output, (SmoothSize > 0));

                //Binarize the data
                CV.Threshold(output, output, _thr, 255, ThresholdTypes.Binary);

                //Find countours of the binary regions
                Seq currentContour;
                using (var storage = new MemStorage())
                using (var scanner = CV.StartFindContours(output, storage, Contour.HeaderSize, ContourRetrieval.External, ContourApproximation.ChainApproxNone, new OpenCV.Net.Point(0, 0)))
                {
                    while ((currentContour = scanner.FindNextContour()) != null)
                    {
                        //calculate the number of pixels inside the contour
                        contourArea = CV.ContourArea(currentContour, SeqSlice.WholeSeq);
                        CV.DrawContours(output, currentContour, new Scalar(80), new Scalar(80), 1, -1);
                        //if number of pixels fit the expected for the fly, calculate the distribution moments
                        if (contourArea > MinArea && contourArea < MaxArea)
                        {
                            float[] param = new float[4];
                            AngleVal ori = new AngleVal() { Radians = 0f };
                            scanner.SubstituteContour(null);
                            //calculate the pixel distribution moments
                            moments = new Moments(currentContour);
                            if (moments.M00 > 0)
                            {
                                //transform moments into X,Y and orientation
                                param[0] = Convert.ToSingle(moments.M10 / moments.M00);
                                param[1] = Convert.ToSingle(moments.M01 / moments.M00);
                                ori.Radians = Convert.ToSingle(0.5 * Math.Atan2(2 * (moments.M11 / moments.M00 - param[0] * param[1]),
                                    (moments.M20 / moments.M00 - param[0] * param[0])
                                    - (moments.M02 / moments.M00 - param[1] * param[1])));
                                param[2] = ori.Degrees;

                                if (moments.M00 > 5)
                                {
                                    //save head coordenates
                                    elipse = CV.FitEllipse2(currentContour);
                                    param[3] = elipse.Size.Height; // param[3] is major axis length
                                }
                                else
                                {
                                    param[3] = 0f; // param[3] is major axis length
                                }
                            }
                            // save blob features into fly params list
                            flyParams.Add(param);
                            bfArea.Add(contourArea);
                            //Console.WriteLine("contour area {0}", contourArea);
                        }
                    }
                }

                if (bfArea.Count() > 0)
                {
                    // Cut extra blobs and sort by size
                    List<double> newBfArea = bfArea.ToList();
                    newBfArea.Sort();
                    newBfArea.Reverse(); //change to descending
                    List<float[]> tmpFlyParams = new List<float[]>(nFlies);

                    for (int i = 0; i < Math.Min(bfArea.Count(),nFlies); i++)
                    {
                        float[] param = new float[4];
                        float[] newparam = new float[5];
                        float[] prevparam = new float[5];
                        AngleVal ori = new AngleVal() { Radians = 0f };

                        int cIdx = bfArea.FindIndex(x => x == newBfArea.ElementAt(i));
                        
                        // Correct Orientation
                        param = flyParams.ElementAt(cIdx);
                        prevparam = trackPreviousResult.ElementAt(i);
                        ori.Degrees = param[2];
                        ori = HeadTailOrientation(param[0], param[1], ori, prevparam);
                        ori = CorrectedOrientation(ori, prevparam[2]);

                        newparam[0] = param[0];
                        newparam[1] = param[1];
                        newparam[2] = ori.Degrees;

                        // Get Fly Head
                        if (param[3] > 0)
                        {
                            newparam[3] = param[0] + Convert.ToSingle(Math.Cos(ori.Radians)) * param[3] / 2f; // param[3] is major axis length
                            newparam[4] = param[1] + Convert.ToSingle(Math.Sin(ori.Radians)) * param[3] / 2f; // param[3] is major axis length
                        }
                        else
                        {
                            newparam[3] = param[0];
                            newparam[4] = param[1];
                        }

                        tmpFlyParams.Add(newparam);
                    }

                    flyParams = tmpFlyParams;
                    bfArea = newBfArea;

                    float[] plotparam = flyParams.ElementAt(flyParams.Count()-1);
                    CV.Line(output, new Point(Convert.ToInt32(plotparam[0]), Convert.ToInt32(plotparam[1])), new Point(Convert.ToInt32(plotparam[3]), Convert.ToInt32(plotparam[4])), new Scalar(125));

                }
                else
                {
                    flyParams = new List<float[]>();
                }

                if (boolDisplayTrackingResult)
                imVis.Show(output.Clone());

            }
            while (flyParams.Count() < nFlies)
            {
                flyParams.Add(new float[5]);
            }
            return flyParams;
        }



        //Auxiliar Functions ------------------------------------------------------------------------
        public AngleVal CorrectedOrientation(AngleVal or, float orp)
        {
            //number of pi rotations in one frame
            int npi = (int)Math.Round((orp - or.Degrees) / 180f);

            //remove or add appropriate pi rotations
            if (Math.Abs(Mod2pi(or.Degrees - orp + 180f) / 2) > Math.Abs(Mod2pi(or.Degrees - orp)))
            {
                or.Degrees += (npi - npi % 2) * 180f;
            }
            else
            {
                or.Degrees += npi * 180f;
            }

            return or;
        }
                
        public float Mod2pi(float or)
        {
            //Make the angle modulus 2pi 
            while (or <= -180f) or += 2 * (float)180f;
            while (or > 180f) or -= 2 * (float)180f;
            return or;
        }

        public AngleVal HeadTailOrientation (float X, float Y, AngleVal or, float[] prev)
        {
            int idx = 0;
            int check = 1;
            float average = 0f;
            float norm;
            //Point2f previous = new Point2f(trackPreviousResult[0] - trackPreviousResult[3], trackPreviousResult[1] - trackPreviousResult[4]);
            //Point2f current = new Point2f(trackPreviousResult[0] - X, trackPreviousResult[1] - Y);
            Point2f previous = new Point2f(prev[0] - prev[3], prev[1] - prev[4]);
            Point2f current = new Point2f(prev[0] - X, prev[1] - Y);
            AngleVal theta = new AngleVal() { Radians = 0f };

            norm = (float)Math.Sqrt((previous.X - current.X) * (previous.X - current.X) +
                (previous.Y - current.Y) * (previous.Y - current.Y));
            theta.Radians = (float)Math.Acos((previous.X * current.X + previous.Y * current.Y) / norm);

            while (idx < 9)
            {
                trackMovementDirection[idx] = trackMovementDirection[idx + 1];
                idx += 1;
                check = check * (int)trackMovementDirection[idx];
                average += trackMovementDirection[idx];
            }

            if (Math.Abs(theta.Degrees) > 90) { trackMovementDirection[9] = -1; } else { trackMovementDirection[9] = 1; };

            average += trackMovementDirection[9];

            if (average < 0 && check!=0)
            {
                or.Degrees += 180;
                trackMovementDirection[9] = 0;
            }

            return or;
        }

        public IplImage BackgroundSubtraction(IplImage output, bool boolSmooth)
        {
            //smooth the Current Image
            if (boolSmooth)
                CV.Smooth(output, Background, SmoothMethod.Median, SmoothSize);

            //subtract Background from frame
            CV.Sub(Background, output, output);

            return output;
        }
        //Auxiliar Functions ------------------------------------------------------------------------



        protected override void Process(T data)
        {
            List<float[]> trackCurrentResult = new List<float[]>();
            using (IplImage input = data.image)
            {
                trackCurrentResult = ObtainTrackingData(input);
            }

            
            // param[0] - X Coord (Arena Pixels);
            // param[1] - Y Coord (Arena Pixels);
            // param[2] - Orientation (Degrees);
            // param[3] - X Head Coord (Arena Pixels);
            // param[4] - Y Head Coord (Arena Pixels);

            if (nFlies == 1)
            {
                float[] param = new float[5];
                param = trackCurrentResult.ElementAt(0);
                this.Send<MovementData>(new MovementData(data.ID, data.source, new float[] {
                    param[0], param[1],
                    param[2],
                    ((param[0]-param[0])*(float)Math.Cos(Math.PI*param[2]/180) + (param[1]-param[1])*(float)Math.Sin(Math.PI*param[2]/180))*data.frameRate ,
                    (-(param[0]-param[0])*(float)Math.Sin(Math.PI*param[2]/180) + (param[1]-param[1])*(float)Math.Cos(Math.PI*param[2]/180))*data.frameRate ,
                    (param[2] - param[2])*data.frameRate },
                        new float[] { param[3], param[4] }, vw._time
                        ));  
            }
            if (nFlies == 2)
            {
                float[] param = new float[5];
                float[] paramM = new float[5];
                param = trackCurrentResult.ElementAt(0); // bigger (most likely female)
                paramM = trackCurrentResult.ElementAt(1); // smaller (most likely male)
                this.Send<MovementData>(new MovementData(data.ID, data.source, new float[] {
                    paramM[0], paramM[1],
                    paramM[2],
                    ((paramM[0]-paramM[0])*(float)Math.Cos(Math.PI*paramM[2]/180) + (paramM[1]-paramM[1])*(float)Math.Sin(Math.PI*paramM[2]/180))*data.frameRate ,
                    (-(paramM[0]-paramM[0])*(float)Math.Sin(Math.PI*paramM[2]/180) + (paramM[1]-paramM[1])*(float)Math.Cos(Math.PI*paramM[2]/180))*data.frameRate ,
                    (paramM[2] - paramM[2])*data.frameRate },
                        new float[] { paramM[3], paramM[4] },
                        new float[] { param[0], param[1], param[2], param[3], param[4] }, vw._time
                        ));
                Console.WriteLine("{0} {1}", paramM[0], param[0]);
            }
               
            trackPreviousResult = trackCurrentResult;
            data.image.Dispose();
        }

        public void OnExit()
        {
            if(vis != null)
            {
                imVis.Unload();
                vis.Dispose();
            }
            this.Abort();
        }

        public void DisposeModule()
        {
            Background.Dispose();
            this.Dispose();
        }
    }
}
