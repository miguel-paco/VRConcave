using FlyVRena2._0.ImageProcessing;
using FlyVRena2._0.VirtualWorld.Subsystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateWithFlyConstantDrift : UpdateService<FilteredData>
    {
        public float rSpeed;
        public string direction;
        public List<Trial> trials;
        public UpdateWithFlyConstantDrift(IServiceContainer wObj, VirtualWorld VW, float rSpeed, string direction, List<Trial> trials) : base(wObj, VW)
        {
            this.rSpeed = rSpeed;
            this.trials = trials;
            this.direction = direction;
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
            /*
            if (this.nameService.name == trials.ElementAt(i).TrialType)
            {

                if (direction == "horizontal")
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.Y = 0f;
                    this.positionService.position.X += rSpeed * (float)time;
                }
                else if (direction == "vertical")
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.X = 0f;
                    this.positionService.position.Y += rSpeed * (float)time;
                }
                else if (direction == "angular")
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.X = 0f;
                    this.positionService.position.Y = 0f;
                    this.positionService.rotation.Z += rSpeed * (float)time;
                }
                else
                {
                    this.positionService.position.Z = 1f;
                    this.positionService.position.X = 0f;
                    this.positionService.position.Y = 0f;
                }

            }
            else
            {
                this.positionService.position.Z = 0f;
                this.positionService.position.X = 0f;
                this.positionService.position.Y = 0f;
            }
            */


            if (timeT - auxTime >= trials.ElementAt(i).TrialDuration)
            {
                auxTime = timeT;
                if (i < trials.Count-1)
                    i +=1;
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
