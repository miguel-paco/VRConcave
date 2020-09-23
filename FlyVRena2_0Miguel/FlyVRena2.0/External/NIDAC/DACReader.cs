using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NationalInstruments.DAQmx;
using Sardine.Core;

namespace FlyVRena2._0.External.NIDAC
{
    public class DACReader : Module
    {
        private string _device = "Dev1";
        private NationalInstruments.DAQmx.Task niTask;
        private double[] _value = { Double.NaN, Double.NaN, Double.NaN };
        public double[] Value
        {
            get => _value;
            set
            {
                _value = value;
                Changed("Value");
            }
        }
        private NationalInstruments.DAQmx.AnalogMultiChannelReader channels = null;

        private int minVoltage = -10;
        private int maxVoltage = 10;

        private string _port1 = "ai0";
        private string _port2 = "ai4";
        private string _port3 = "ai5";
        //private string triggerSource = "";

        public bool dacON = true;

        private AsyncCallback analogCallback;
        private double[,] data;

        public DACReader(int minV, int maxV) : base()
        {
            this.minVoltage = minV;
            this.maxVoltage = maxV;

            niTask = new NationalInstruments.DAQmx.Task();
            try
            {
                niTask.AIChannels.CreateVoltageChannel(_device + "/" + _port1, "ElectrodeChannel", (AITerminalConfiguration)(-1), minVoltage, maxVoltage, AIVoltageUnits.Volts);
                niTask.AIChannels.CreateVoltageChannel(_device + "/" + _port2, "PhotodiodeChannel", (AITerminalConfiguration)(-1), minVoltage, maxVoltage, AIVoltageUnits.Volts);
                niTask.AIChannels.CreateVoltageChannel(_device + "/" + _port3, "TreadmillClockChannel", (AITerminalConfiguration)(-1), minVoltage, maxVoltage, AIVoltageUnits.Volts);
                niTask.Timing.ConfigureSampleClock("OnboardClock", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);
                niTask.Control(TaskAction.Verify);
                channels = new AnalogMultiChannelReader(niTask.Stream);
                channels.SynchronizeCallbacks = true;
                analogCallback = new AsyncCallback(AnalogInCallback);
               // niTask.AIChannels.All.AutoZeroMode = AIAutoZeroMode.EverySample;
            }
            catch
            {
                dacON = false;
            }
        }

        ulong ID = 0;
        public void Init()
        {
            niTask.Start();
            channels.BeginReadMultiSample(1000, analogCallback, niTask);
        }

        private void AnalogInCallback(IAsyncResult ar)
        {
            try
            {
                if (niTask != null && !niTask.IsDone && niTask == ar.AsyncState)
                {
                    data = channels.EndReadMultiSample(ar);
                    Send<DACData>(new DACData(data, ID));
                    ID++;
                    channels.BeginReadMultiSample(1000, analogCallback, niTask);
                }
            }
            catch
            {
                //MessageBox.Show("DAC Recording Failed");
            }
        }

        protected override void Work(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                this.Dispose();
            }
        }

        public void OnExit()
        {
            niTask.Stop();
            this.Abort();
            this.Dispose();
        }

        public void DisposeModule()
        {
            niTask.Dispose();
            this.Dispose();
        }
    }
}
