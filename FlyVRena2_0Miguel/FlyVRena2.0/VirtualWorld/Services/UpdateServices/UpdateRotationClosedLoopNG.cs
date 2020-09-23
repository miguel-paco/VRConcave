using FlyVRena2._0.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateRotationClosedLoopNG : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public List<Trial> trials;
        public UpdateRotationClosedLoopNG(IServiceContainer wObj, VirtualWorld VW, long freq) : base(wObj, VW)
        {
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
                this.positionService.rotation.Y = posServ.rotation.Y;
            }
            else
            { }
            if (timeT - auxTime >= trials.ElementAt(i).TrialDuration)
            {
                auxTime = timeT;
                if (i < trials.Count - 1)
                    i += 1;
            }
        }



        protected override void Process(FilteredData data)
        {
            posServ.rotation.Y = -data.position[2];
        }
    }
}
