using FlyVRena2._0.External;
using FlyVRena2._0.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCV.Net;
using FlyVRena2._0.VirtualWorld.Services.DrawServices;
using uEye.Defines.IO;

namespace FlyVRena2._0.VirtualWorld.Services.UpdateServices
{
    public class UpdatePhotodiode : UpdateService<FilteredData>
    {
        private PositionService posServ;
        public float k = 0f; // Check if at least one FilteredData was generated
        public Coordinates centroid;

        // Specific Variables
        public float counter = 0f;
        public float constant = 1000f;
        public float photodiodeShadowTimer = 100; // milliseconds
        public bool photoStatus = false;



        public UpdatePhotodiode(IServiceContainer wObj, VirtualWorld VW, float photodiodeShadowTimer) : base(wObj, VW)
        {
            posServ = new PositionService();
            this.photodiodeShadowTimer = photodiodeShadowTimer;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        // Open Loop Section
        public override void Update(double time)
        {
            if (k == 1)
            {
                counter += 1;

                if (counter == photodiodeShadowTimer)
                {
                    photoStatus = !photoStatus;
                     counter = 0;
                     constant = -constant;
                     this.positionService.position.X += constant;
                     this.positionService.position.Y += constant;
                }
            }
        }


        protected override void Process(FilteredData data)
        {
            if (k == 0)
            {
                k = 1;
            }
        }
    }
}
