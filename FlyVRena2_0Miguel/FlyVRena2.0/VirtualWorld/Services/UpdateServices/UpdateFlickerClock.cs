using FlyVRena2._0.ImageProcessing;
using FlyVRena2._0.VirtualWorld.Subsystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdateFlickerClock : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public ulong freq;
        public UpdateFlickerClock(IServiceContainer wObj, VirtualWorld VW, long freq) : base(wObj, VW)
        {
            this.freq = (ulong)freq;
            posServ = new PositionService();
            posServ.rotation.X = 0f;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        public override void Update(double time)
        {
            this.positionService.rotation.X = posServ.rotation.X;
        }

        private long counter = 0;
        protected override void Process(FilteredData data)
        {
            if (data.ID == 1)
            {
                //posServ.rotation.X = 1.5708f;
            }
            counter = counter + 1;
            if (data.ID % freq == 0)
            {
                counter = 0;
                if (posServ.rotation.X == 0.0f)
                {
                    posServ.rotation.X = 1.5708f;
                }
                else
                {
                    posServ.rotation.X = 0.0f;
                }
            }
        }
    }
}
