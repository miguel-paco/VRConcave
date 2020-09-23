using FlyVRena2._0.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateOscilatingDrift : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public float rSpeed;
        public float amp;
        public string direction;
        public List<Trial> trials;
        public UpdateOscilatingDrift(IServiceContainer wObj, VirtualWorld VW, float rSpeed, float amp, string direction, List<Trial> trials) : base(wObj, VW)
        {
            this.rSpeed = rSpeed;
            this.amp = amp;
            this.direction = direction;
            this.trials = trials;
            posServ = new PositionService();
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
                if (direction == "horizontal")
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.Y = 0f;
                    this.positionService.position.X = amp * (float)Math.Sin(rSpeed * timeT);
                }
                else if (direction == "vertical")
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.X = 0f;
                    this.positionService.position.Y = amp * (float)Math.Sin(rSpeed * timeT);
                }
                else if (direction == "angular")
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.X = 0f;
                    this.positionService.position.Y = 0f;
                    this.positionService.rotation.Z = amp * (float)Math.Sin(rSpeed * timeT);
                }
                else
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.X = 0f;
                    this.positionService.position.Y = 0f;
                }
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
        }
    }
}
