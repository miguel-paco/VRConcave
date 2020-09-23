using FlyVRena2._0.External;
using FlyVRena2._0.Visualizers;
using OpenCV.Net;
using OpenTK.Graphics.ES20;
using Sardine.Core;
using System;
using System.Diagnostics;

namespace FlyVRena2._0.ImageProcessing
{
    public class FastTracking<T> : Module<T> where T : Frame
    {
        Stopwatch stopWatch = new Stopwatch(); // Check process time auxiliar variable

        bool boolDisplayTrackingResult;
        private float[] trackPreviousResult = new float[5];
        // trackPreviousResult[0] - X Coord (Arena Pixels);
        // trackPreviousResult[1] - Y Coord (Arena Pixels);
        // trackPreviousResult[2] - Orientation (Degrees);
        private IplImage Background;
        
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


        public FastTracking(Frame mask, int minA, int maxA, int thr, int smooth, bool boolDisplayTrackingResult)
        {
            //Initialize with pre-defined parameters. Modify and re-compile to alter tracking parameters. 
            MinArea = minA;
            MaxArea = maxA;
            Thr = thr;
            SmoothSize = smooth;
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
                                    moments.M20 / moments.M00 - trackResult[0] * trackResult[0])
                                    - (moments.M02 / moments.M00 - trackResult[1] * trackResult[1]));

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

                orientation = CorrectedOrientation(orientation, trackPreviousResult[2]);

                trackResult[2] = orientation.Degrees;

                // Find Fly Head
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

                CV.Circle(output, new Point(Convert.ToInt32(trackResult[3] + Convert.ToSingle(Math.Cos(orientation.Radians)) * 3), Convert.ToInt32(trackResult[4] + Convert.ToSingle(Math.Cos(orientation.Radians)) * 3)), 1, new Scalar(255,0,0));


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

        //Make the angle modulus 2pi 
        public float Mod2pi(float or)
        {
            while (or <= -180f) or += 2 * (float)180f;
            while (or > 180f) or -= 2 * (float)180f;
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
                trackCurrentResult = ObtainTrackingData(input);
            }

            this.Send<MovementData>(new MovementData(data.ID, data.source, new float[] {
                trackCurrentResult[0],
                trackCurrentResult[1],
                trackCurrentResult[2],
                ((trackCurrentResult[0]-trackPreviousResult[0])*(float)Math.Cos(Math.PI*trackCurrentResult[2]/180) + (trackCurrentResult[1]-trackPreviousResult[1])*(float)Math.Sin(Math.PI*trackCurrentResult[2]/180))*data.frameRate ,
                (-(trackCurrentResult[0]-trackPreviousResult[0])*(float)Math.Sin(Math.PI*trackCurrentResult[2]/180) + (trackCurrentResult[1]-trackPreviousResult[1])*(float)Math.Cos(Math.PI*trackCurrentResult[2]/180))*data.frameRate ,
                (trackCurrentResult[2] - trackPreviousResult[2])*data.frameRate },
                new float[] { trackCurrentResult[3], trackCurrentResult[4]
                }));
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
