using FlyVRena2._0.External;
using FlyVRena2._0.Visualizers;
using OpenCV.Net;
using OpenTK.Graphics.ES20;
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
        private float[] trackPreviousResult = new float[5] { 0, 0, 0, 0, 0 };
        // trackPreviousResult[0] - X Coord (Arena Pixels);
        // trackPreviousResult[1] - Y Coord (Arena Pixels);
        // trackPreviousResult[2] - Orientation (Degrees);
        // trackPreviousResult[3] - X Head Coord (Arena Pixels);
        // trackPreviousResult[4] - Y Head Coord (Arena Pixels);
        private float[] trackPreviousResult2 = new float[5] { 0, 0, 0, 0, 0 };
        private IplImage Background;
        private float[] trackMovementDirection = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private FlyVRena2._0.VirtualWorld.VirtualWorld vw;

        private bool _twoflies = false;
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
            _twoflies = twoflies;
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
        
        // Single Fly
        public float[] ObtainTrackingData(IplImage output)
        {
            float[] trackResult = new float[5] { 0, 0, 0, 0, 0};

            if (Background == null) // if no first frame
            {
                Background = output.Clone();
                return trackResult;
            }
            else
            {
                float majoraxis = 0f;
                double contourArea;
                RotatedRect elipse;
                Moments moments;
                Point2f head;
                AngleVal orientation = new AngleVal() { Radians = 0f };

                output = BackgroundSubtraction(output, (SmoothSize>0));
                
                //Binarize the data
                CV.Threshold(output, output, _thr, 255, ThresholdTypes.Binary);

                //Find countours of the binary regions
                Seq currentContour;
                using (var storage = new MemStorage())
                using (var scanner = CV.StartFindContours(output, storage, Contour.HeaderSize, ContourRetrieval.External, ContourApproximation.ChainApproxNone, new OpenCV.Net.Point(0, 0)))
                {
                    double bfArea = 0;
                    while ((currentContour = scanner.FindNextContour()) != null)
                    {
                        //Calculate the number of pixels inside the contour
                        contourArea = CV.ContourArea(currentContour, SeqSlice.WholeSeq);
                        CV.DrawContours(output, currentContour, new Scalar(80), new Scalar(80), 1, -1);
                        //Console.WriteLine("{0}", contourArea);
                        //if number of pixels fit the expected for the fly, calculate the distribution moments
                        if (contourArea > bfArea && (contourArea > MinArea && contourArea < MaxArea))
                        {
                            scanner.SubstituteContour(null);
                            //calculate the pixel distribution moments
                            moments = new Moments(currentContour);
                            if (moments.M00 > 0)
                            {
                                //transform moments into X,Y and orentation   
                                trackResult[0] = Convert.ToSingle(moments.M10 / moments.M00);
                                trackResult[1] = Convert.ToSingle(moments.M01 / moments.M00);
                                orientation.Radians = Convert.ToSingle(0.5 * Math.Atan2(2 * (moments.M11 / moments.M00 - trackResult[0] * trackResult[1]),
                                    (moments.M20 / moments.M00 - trackResult[0] * trackResult[0])
                                    - (moments.M02 / moments.M00 - trackResult[1] * trackResult[1])));

                                if (moments.M00 > 5)
                                {
                                    //save head coordenates
                                    elipse = CV.FitEllipse2(currentContour);
                                    majoraxis = elipse.Size.Height;
                                }
                                else
                                {
                                    majoraxis = 0f;
                                }
                            }
                            CV.DrawContours(output, currentContour, new Scalar(255), new Scalar(255), 1, -1);        
                        }
                        bfArea = contourArea;
                    }
                }

                orientation = HeadTailOrientation(trackResult[0], trackResult[1], orientation, trackPreviousResult);

                orientation = CorrectedOrientation(orientation, trackPreviousResult[2]);

                trackResult[2] = orientation.Degrees;

                // Find Fly Head #1
                if (majoraxis > 0)
                {
                    trackResult[3] = trackResult[0] + Convert.ToSingle(Math.Cos(orientation.Radians)) * majoraxis / 2f;
                    trackResult[4] = trackResult[1] + Convert.ToSingle(Math.Sin(orientation.Radians)) * majoraxis / 2f;
                }
                else
                {
                    trackResult[3] = trackResult[0];
                    trackResult[4] = trackResult[1];
                }

                //CV.Circle(output, new Point(Convert.ToInt32(trackResult[3]), Convert.ToInt32(trackResult[4])), 0, new Scalar(125));
                CV.Line(output, new Point(Convert.ToInt32(trackResult[0]), Convert.ToInt32(trackResult[1])), new Point(Convert.ToInt32(trackResult[3]), Convert.ToInt32(trackResult[4])), new Scalar(125));

                if (boolDisplayTrackingResult)
                    imVis.Show(output.Clone());

                return trackResult;
            }
        }

        public float[] ObtainTrackingData2Flies(IplImage output)
        {
            float[] trackResult = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            float[] trackResult1 = new float[5] { 0, 0, 0, 0, 0 };
            float[] trackResult2 = new float[5] { 0, 0, 0, 0, 0 };

            if (Background == null) // if no first frame
            {
                Background = output.Clone();
                return trackResult;
            }
            else
            {
                float majoraxis1 = 0f;
                float majoraxis2 = 0f;
                double contourArea;
                RotatedRect elipse;
                Moments moments;
                Point2f head;
                AngleVal orientation1 = new AngleVal() { Radians = 0f };
                AngleVal orientation2 = new AngleVal() { Radians = 0f };

                output = BackgroundSubtraction(output, (SmoothSize > 0));

                //Binarize the data
                CV.Threshold(output, output, _thr, 255, ThresholdTypes.Binary);

                //Find countours of the binary regions
                Seq currentContour;
                using (var storage = new MemStorage())
                using (var scanner = CV.StartFindContours(output, storage, Contour.HeaderSize, ContourRetrieval.External, ContourApproximation.ChainApproxNone, new OpenCV.Net.Point(0, 0)))
                {
                    double bfArea = 0;
                    while ((currentContour = scanner.FindNextContour()) != null)
                    {
                        //Calculate the number of pixels inside the contour
                        contourArea = CV.ContourArea(currentContour, SeqSlice.WholeSeq);
                        CV.DrawContours(output, currentContour, new Scalar(80), new Scalar(80), 1, -1);
                        //Console.WriteLine("{0}", contourArea);
                        //if number of pixels fit the expected for the fly, calculate the distribution moments
                        if (contourArea > bfArea && (contourArea > MinArea && contourArea < MaxArea))
                        {
                            scanner.SubstituteContour(null);
                            //calculate the pixel distribution moments
                            moments = new Moments(currentContour);
                            if (moments.M00 > 0)
                            {
                                trackResult2 = trackResult1;
                                majoraxis2 = majoraxis1;
                                orientation2 = orientation1;
                                //transform moments into X,Y and orentation   
                                trackResult1[0] = Convert.ToSingle(moments.M10 / moments.M00);
                                trackResult1[1] = Convert.ToSingle(moments.M01 / moments.M00);
                                orientation1.Radians = Convert.ToSingle(0.5 * Math.Atan2(2 * (moments.M11 / moments.M00 - trackResult1[0] * trackResult1[1]),
                                    (moments.M20 / moments.M00 - trackResult1[0] * trackResult1[0])
                                    - (moments.M02 / moments.M00 - trackResult1[1] * trackResult1[1])));

                                if (moments.M00 > 5)
                                {
                                    //save head coordenates
                                    elipse = CV.FitEllipse2(currentContour);
                                    majoraxis1 = elipse.Size.Height;
                                }
                                else
                                {
                                    majoraxis1 = 0f;
                                }
                            }
                            CV.DrawContours(output, currentContour, new Scalar(255), new Scalar(255), 1, -1);
                        }
                        bfArea = contourArea;
                    }
                }

                orientation1 = HeadTailOrientation(trackResult1[0], trackResult1[1], orientation1, trackPreviousResult);
                orientation2 = HeadTailOrientation(trackResult2[0], trackResult2[1], orientation2, trackPreviousResult2);

                orientation1 = CorrectedOrientation(orientation1, trackPreviousResult[2]);
                orientation2 = CorrectedOrientation(orientation2, trackPreviousResult2[2]);

                trackResult1[2] = orientation1.Degrees;
                trackResult2[2] = orientation2.Degrees;

                // Find Fly Head #1
                if (majoraxis1 > 0)
                {
                    trackResult1[3] = trackResult1[0] + Convert.ToSingle(Math.Cos(orientation1.Radians)) * majoraxis1 / 2f;
                    trackResult[4] = trackResult1[1] + Convert.ToSingle(Math.Sin(orientation1.Radians)) * majoraxis1 / 2f;
                }
                else
                {
                    trackResult1[3] = trackResult1[0];
                    trackResult1[4] = trackResult1[1];
                }
                // Find Fly Head #2
                if (majoraxis2 > 0)
                {
                    trackResult2[3] = trackResult2[0] + Convert.ToSingle(Math.Cos(orientation2.Radians)) * majoraxis2 / 2f;
                    trackResult2[4] = trackResult2[1] + Convert.ToSingle(Math.Sin(orientation2.Radians)) * majoraxis2 / 2f;
                }
                else
                {
                    trackResult2[3] = trackResult2[0];
                    trackResult2[4] = trackResult2[1];
                }

                // Save the tracked blobs
                // Male is the smallest blob "2" and female the biggest "1"
                trackResult[0] = trackResult2[0]; // Male X Coord(Arena Pixels);
                trackResult[1] = trackResult2[1]; // Male Y Coord(Arena Pixels);
                trackResult[2] = trackResult2[2]; // Male Orientation (Degrees);
                trackResult[3] = trackResult2[3]; // Male X Head Coord (Arena Pixels);
                trackResult[4] = trackResult2[4]; // Male Y Head Coord (Arena Pixels);
                trackResult[5] = trackResult1[0]; // Female X Coord(Arena Pixels);
                trackResult[6] = trackResult1[1]; // Female Y Coord(Arena Pixels);
                trackResult[7] = trackResult1[2]; // Female Orientation (Degrees);
                trackResult[8] = trackResult1[3]; // Female X Head Coord (Arena Pixels);
                trackResult[9] = trackResult1[4]; // Female X Head Coord (Arena Pixels);

                //CV.Circle(output, new Point(Convert.ToInt32(trackResult[3]), Convert.ToInt32(trackResult[4])), 0, new Scalar(125));
                CV.Line(output, new Point(Convert.ToInt32(trackResult[0]), Convert.ToInt32(trackResult[1])), new Point(Convert.ToInt32(trackResult[3]), Convert.ToInt32(trackResult[4])), new Scalar(125));

                if (boolDisplayTrackingResult)
                    imVis.Show(output.Clone());

                return trackResult;
            }
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
            float[] trackCurrentResult;
            using (IplImage input = data.image)
            {
                if (_twoflies)
                {
                    trackCurrentResult = ObtainTrackingData2Flies(input);                    

                }
                else
                {
                    trackCurrentResult = ObtainTrackingData(input);
                }
            }

            this.Send<MovementData>(new MovementData(data.ID, data.source, new float[] {
                trackCurrentResult[0],
                trackCurrentResult[1],
                trackCurrentResult[2],
                ((trackCurrentResult[0]-trackPreviousResult[0])*(float)Math.Cos(Math.PI*trackCurrentResult[2]/180) + (trackCurrentResult[1]-trackPreviousResult[1])*(float)Math.Sin(Math.PI*trackCurrentResult[2]/180))*data.frameRate ,
                (-(trackCurrentResult[0]-trackPreviousResult[0])*(float)Math.Sin(Math.PI*trackCurrentResult[2]/180) + (trackCurrentResult[1]-trackPreviousResult[1])*(float)Math.Cos(Math.PI*trackCurrentResult[2]/180))*data.frameRate ,
                (trackCurrentResult[2] - trackPreviousResult[2])*data.frameRate },
                new float[] { trackCurrentResult[3], trackCurrentResult[4] }, vw._time
                ));
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
