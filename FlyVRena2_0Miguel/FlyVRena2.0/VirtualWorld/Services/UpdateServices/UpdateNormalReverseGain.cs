using FlyVRena2._0.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateNormalReverseGain : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public List<Trial> trials;
        public float gain;
        public UpdateNormalReverseGain(IServiceContainer wObj, VirtualWorld VW, float gain, List<Trial> trials) : base(wObj, VW)
        {
            posServ = new PositionService();
            this.trials = trials;
            this.gain = gain;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        double auxTime = 0;
        int i = 0;
        double timeT = 0;
        public override void Update(double time)
        {
            timeT += time;
            if (this.nameService.name == trials.ElementAt(i).TrialType)
            {
                this.positionService.position.Z = 1f;
                this.positionService.position.X = 0f;
                this.positionService.position.Y = 0f;
                
            }
            else
                this.positionService.position.Z = -1;


            if (timeT - auxTime >= trials.ElementAt(i).TrialDuration)
            {
                auxTime = timeT;
                if (i < trials.Count - 1)
                    i += 1;
            }
        }

        int k = 1;
        protected override void Process(FilteredData data)
        {
            if (k == 1)
            {
                k = 0;
                timeT = 0;
            }
            this.positionService.rotation.Z += gain * data.velocity[2];
            //posServ.rotation.Z = d;
        }
    }
}
