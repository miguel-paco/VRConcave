using FlyVRena2._0.ImageProcessing;
using FlyVRena2._0.VirtualWorld.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class RFMappingRotDotStim : UpdateService<FilteredData>
    {
        float xC = 20;
        float yC = 0;
        float amplitude;
        float speed;
        float size;
        float tTime;
        public RFMappingRotDotStim(IServiceContainer wObj, VirtualWorld VW, float amplitude, float size, float speed, float tTime) : base(wObj, VW)
        {
            this.amplitude = amplitude;
            this.size = size;
            this.speed = speed;
            this.tTime = tTime;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        double auxTime = 0;
        double timeT = 0;
        int i = 0;
        int j = 0;
        float xCaux = 0;
        float yCaux = 0;
        public override void Update(double time)
        {
            this.positionService.scale = (float)Math.Tan(size * Math.PI / 180f) * (float)Math.Sqrt(xC * xC + yC * yC);
            float amp = (float)Math.Tan(amplitude*Math.PI/180f) * (float)Math.Sqrt(xC * xC + yC * yC);
            this.positionService.position.Y = amp * (float)Math.Cos(speed * timeT) + yC;
            this.positionService.position.X = amp * (float)Math.Sin(speed * timeT) + xC;
            timeT += time;

            if (timeT - auxTime >= tTime)
            {
                i = i + 1;
                auxTime = timeT;
                if (i<12)
                {
                    xCaux = xC;
                    yCaux = yC;
                    xC = xCaux * (float)Math.Cos(15 * Math.PI / 180f) - yCaux * (float)Math.Sin(15 * Math.PI / 180f);
                    yC = yCaux * (float)Math.Cos(15 * Math.PI / 180f) + xCaux * (float)Math.Sin(15 * Math.PI / 180f);
                }
                else if(j < 10)
                {
                    i = 0;
                    j = j + 1;
                    yC = 0;
                    xC = 20 + 20 * j;
                }
                else
                {
                    i = 0;
                    j = 0;
                    xC = 20;
                    yC = 0;
                }
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
