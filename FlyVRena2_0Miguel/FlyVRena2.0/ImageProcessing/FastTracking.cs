using FlyVRena2._0.External;
using FlyVRena2._0.Visualizers;
using OpenCV.Net;
using Sardine.Core;
using System;
using System.Diagnostics;

namespace FlyVRena2._0.ImageProcessing
{
    public class FastTracking<T> : Module<T> where T : Frame
    {
        Stopwatch stopWatch = new Stopwatch(); // Check process time auxiliar variable

        
        bool disp; // Bool to turn on or off the Tracking Result Display
        private float[] prevTrack = new float[5]; // Value to save the old tracking value
        private IplImage Mask; // Image to use as support on the tresholding methods (either as a reference to subtract or to save current image)
        
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


        public FastTracking(Frame mask, int minA, int maxA, int thr, int smooth, bool disp)
        {
            //Initialize with pre-defined parameters. Modify and re-compile to alter tracking parameters. 
            MinArea = minA;
            MaxArea = maxA;
            Thr = thr;
            SmoothSize = smooth;
            this.disp = disp;
            Mask = mask.image; // In background subtraction method, this will be the first adquired frame 
            if (disp)
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
        
        public float[] ObtainTrackingData(IplImage output)
        {
            float[] param = new float[5] { 0, 0, 0, 0, 0};

            if (Mask == null) // if no first frame
            {
                Mask = output.Clone();
                return param;
            }
            else
            {
                float majoraxis = 0f;
                double contourArea;
                RotatedRect elipse;
                Moments moments;

                //// Smoothing Tresholding ------------------------------------------------------------------------
                ////save the Current Image on the Mask
                //Mask = output.Clone();

                ////smooth the Current Image
                //CV.Smooth(output, output, SmoothMethod.Median, SmoothSize);

                ////subtract mask from frame
                //CV.Sub(output, Mask, output);
                //// ----------------------------------------------------------------------------------------------

                //// Background Subtraction -----------------------------------------------------------------------
                //subtract mask from frame
                CV.Sub(Mask, output, output);
                //// ----------------------------------------------------------------------------------------------

                // Find Fly -------------------------------------------------------------------------------------
                
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
                                param[0] = Convert.ToSingle(moments.M10 / moments.M00);
                                param[1] = Convert.ToSingle(moments.M01 / moments.M00);
                                param[2] = 180f * Convert.ToSingle(0.5 * Math.Atan2(2 * (moments.M11 / moments.M00 - param[0] * param[1]), (moments.M20 / moments.M00 - param[0] * param[0]) - (moments.M02 / moments.M00 - param[1] * param[1]))) / Convert.ToSingle(Math.PI);

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
                                param[3] = majoraxis;

                            }
                            CV.DrawContours(output, currentContour, new Scalar(255), new Scalar(255), 1, -1);
                            
                            
                        }
                        bfArea = contourArea;
                    }
                }

                // ----------------------------------------------------------------------------------------------

                // Correct Orientation Based on Previous Tracking
                param[2] = CorrectedOrientation(param[2], prevTrack[2]);

                // Find Fly Head
                if (majoraxis > 0)
                {
                    param[3] = param[0] + Convert.ToSingle(Math.Cos(param[2] * Math.PI / 180f)) * majoraxis / 2f;
                    param[4] = param[1] + Convert.ToSingle(Math.Sin(param[2] * Math.PI / 180f)) * majoraxis / 2f;
                }
                else
                {
                    param[3] = param[0];
                    param[4] = param[1];
                }
                CV.Circle(output, new Point(Convert.ToInt32(param[3] + Convert.ToSingle(Math.Cos(param[2] * Math.PI / 180f)) * 3), Convert.ToInt32(param[4] + Convert.ToSingle(Math.Sin(param[2] * Math.PI / 180f)) * 3)), 1, new Scalar(255,0,0));

                if (disp)
                    imVis.Show(output.Clone());

                return param;
            }
        }

        //correct orientation to try to avoid jumps and make it contnuous (no modulus)
        public float CorrectedOrientation(float or, float orp)
        {
            //number of pi rotations in one frame
            int npi = (int)Math.Round((orp - or) / 180f);

            //remove or add appropriate pi rotations
            if (Math.Abs(Mod2pi(or - orp + 180f) / 2) > Math.Abs(Mod2pi(or - orp)))
            {
                return or + (npi - npi % 2) * 180f;
            }
            else
            {
                return or + npi * 180f;
            }
        }

        //Make the angle modulus 2pi 
        public float Mod2pi(float or)
        {
            while (or <= -180f) or += 2 * (float)180f;
            while (or > 180f) or -= 2 * (float)180f;
            return or;
        }

        protected override void Process(T data)
        {
            float[] currentTracking;
            using (IplImage input = data.image)
            {
                currentTracking = ObtainTrackingData(input);
            }

            this.Send<MovementData>(new MovementData(data.ID, data.source, new float[] {
                currentTracking[0],
                currentTracking[1],
                currentTracking[2],
                ((currentTracking[0]-prevTrack[0])*(float)Math.Cos(Math.PI*currentTracking[2]/180) + (currentTracking[1]-prevTrack[1])*(float)Math.Sin(Math.PI*currentTracking[2]/180))*data.frameRate ,
                (-(currentTracking[0]-prevTrack[0])*(float)Math.Sin(Math.PI*currentTracking[2]/180) + (currentTracking[1]-prevTrack[1])*(float)Math.Cos(Math.PI*currentTracking[2]/180))*data.frameRate ,
                (currentTracking[2] - prevTrack[2])*data.frameRate },
                new float[] { currentTracking[3], currentTracking[4]
                }));
            prevTrack = currentTracking;
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
            Mask.Dispose();
            this.Dispose();
        }
    }
}
