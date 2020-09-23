using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Sardine.Core;

namespace FlyVRena2._0.ImageProcessing
{
    //pass the tracking data through a kalman filter to reduce noise
    public class KalmanFilterTrack<T> : Module<T> where T : MovementData
    {
        private ModelKalman mk;

        private Kalman kal;
        Matrix<float> estimated;

        bool use;

        public float[] pars;
        public KalmanFilterTrack(bool use)
        {
            //initialize new kalman filter with appropriate number of parameters
            kal = new Kalman(6, 3, 0);
            mk = new ModelKalman();
            Matrix<float> state = new Matrix<float>(new float[]
            {
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f
            });

            //define kalman filter according with the kalman model
            kal.CorrectedState = state;
            kal.TransitionMatrix = mk.transitionMatrix;
            kal.MeasurementNoiseCovariance = mk.measurementNoise;
            kal.ProcessNoiseCovariance = mk.processNoise;
            kal.ErrorCovariancePost = mk.errorCovariancePost;
            kal.MeasurementMatrix = mk.measurementMatrix;
            pars = new float[3];
            this.use = use;
        }

        //Filter data and store values

        private void filterPoints(float[] pt)
        {
            //add tracking data as filter state
            mk.state[0, 0] = pt[0];
            mk.state[1, 0] = pt[1];
            mk.state[2, 0] = pt[2];

            //run filter and estimate real position and orientation
            kal.Predict();
            estimated = kal.Correct(mk.GetMeasurement());
            mk.GoToNextState();

            //save estimated position
            pars[0] = estimated[0, 0];
            pars[1] = estimated[1, 0];
            pars[2] = estimated[2, 0] / 5f;
        }

        protected override void Process(T data)
        {
            //filterPoints(data.position);
            Send<FilteredData>(new FilteredData(data.ID, data.source, data.position, data.velocity, data.raw, data.clock));
        }

        //Dispose of all initiated objects
        public void OnExit()
        {
            kal.Dispose();
            mk.Dispose();
            this.Abort();
        }

        public void DisposeModule()
        {
            this.Dispose();
        }
    }
}
