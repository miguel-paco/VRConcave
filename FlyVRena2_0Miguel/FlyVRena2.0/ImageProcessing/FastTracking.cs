using FlyVRena2._0.External;
using FlyVRena2._0.Visualizers;
using OpenCV.Net;
using Sardine.Core;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.ImageProcessing
{
    public class FastTracking<T> : Module<T> where T : Frame
    {
        Stopwatch stopWatch = new Stopwatch(); // Check process time auxiliar variable

        // Define Inputs +++++++++++++++++++++++++++++++++++++++++++++++++++++
        bool disp; // Bool to turn on or off the Tracking Result Display
        private float[] calibValues; // Values to change reference of the values
        private float[] prevTrack = new float[5]; // Value to save the old tracking value
        private IplImage Mask; // Image to use as support on the tresholding methods (either as a reference to subtract or to save current image)
        // Min area within detected contours
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
        // Max area within detected contours
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
        // Treshold Value
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
        // Size of the median smooth filter
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
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // Initialization ====================================================
        public FastTracking(Frame mask, int minA, int maxA, int thr, int smooth, bool disp)
        {
            //Initialize with pre-defined parameters. Modify and re-compile to alter tracking parameters. 
            MinArea = minA;
            MaxArea = maxA;
            Thr = thr;
            SmoothSize = smooth;
            this.disp = disp;
            Mask = mask.image; // As design, this will be the first adquired frame
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
            calibValues = new float[12];
            calibValues[0] = 0.0011f;
            calibValues[1] = -0.7660f;
            calibValues[2] = 0.0000f;
            calibValues[3] = 0.0004f;
            calibValues[4] = 1;
            calibValues[5] = 0.0012f;
            calibValues[6] = -0.0000f;
            calibValues[7] = -0.3676f;
            calibValues[8] = 12.5535f;
            calibValues[9] = 4.6354f + 0.07f;
            calibValues[10] = -12.9056f;
            calibValues[11] = -1.0711f;
        }
        // ===================================================================
        
        public float[] GetParams(IplImage output)
        {
            if (Mask == null) // if no first frame
            {
                Mask = output.Clone();
                return new float[4] { 0, 0, 0, 0 };
            }
            else
            {
                float[] param = new float[4];
                float majoraxis;
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

                //subtract mask from frame
                CV.Sub(Mask, output, output);

                //threshold to binarize the data
                CV.Threshold(output, output, _thr, 255, ThresholdTypes.Binary);

                //find countours of the binary regions
                Seq currentContour;
                using (var storage = new MemStorage())
                using (var scanner = CV.StartFindContours(output, storage, Contour.HeaderSize, ContourRetrieval.External, ContourApproximation.ChainApproxNone, new OpenCV.Net.Point(0, 0)))
                {
                    double bfArea = 0;
                    while ((currentContour = scanner.FindNextContour()) != null)
                    {
                        //calculate the number of pixels inside the contour
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

                if (disp)
                    imVis.Show(output.Clone());

                //Console.WriteLine("{0}", param[0]);
                //return position and orientation
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

        public float[] Calibrate(float[] data)
        {
            float[] rVals = new float[4];

            rVals[0] = data[0]; // Centroid X coord
            rVals[1] = data[1]; // Centroid Y coord
            rVals[2] = data[2]; // Corrected Orientation (deg)
            rVals[3] = data[3]; // MajorAxis
            return rVals;
        }

        public float[] GetHeadCoords(float[] data, IplImage output)
        {
            float[] param = new float[5];
            float majoraxis;

            param[0] = data[0]; // Centroid X coord
            param[1] = data[1]; // Centroid Y coord
            param[2] = data[2]; // Corrected Orientation (deg)
            majoraxis = data[3];

            //find head coordenates (x: param[3]; y: param[4])
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

            return param;
        }

        protected override void Process(T data)
        {
            float[] currentTracking;
            using (IplImage input = data.image)
            {
                currentTracking = GetParams(input);
            }
            currentTracking = Calibrate(currentTracking);
            currentTracking[2] = CorrectedOrientation(currentTracking[2], prevTrack[2]);
            using (IplImage input = data.image)
            {
                currentTracking = GetHeadCoords(currentTracking,input);
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
