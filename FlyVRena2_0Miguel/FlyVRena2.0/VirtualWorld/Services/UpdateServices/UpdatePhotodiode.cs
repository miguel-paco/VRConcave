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
        public float photodiodeShadowFrames = 0;
        public bool photoStatus = false;

        // Load Photodiode
        Photodiode photodiode;
        string photoLvl;

        public UpdatePhotodiode(IServiceContainer wObj, VirtualWorld VW, float photodiodeShadowFrames) : base(wObj, VW)
        {
            posServ = new PositionService();
            this.photodiodeShadowFrames = photodiodeShadowFrames;
            if (VW.update != null)
            {
                VW.update.UpdateServices.Add(this.nameService.name, this);
            }
        }

        // Open Loop Section
        public override void Update(double time)
        {
           
        }


        protected override void Process(FilteredData data)
        {
            counter += 1;
            if (counter == photodiodeShadowFrames)
            {
                counter = 0;
                constant = -constant;
                this.positionService.position.X += constant;
                this.positionService.position.Y += constant;
                photoStatus = !photoStatus;
            }

            photodiode = data.photodiode;
            if (photodiode != null)
            {
                photoLvl = photodiode.Read;
                // SEE PHOTODIODE RESULT
                Console.WriteLine(photoLvl);
            }

            this.Send<PhotoData>(new PhotoData(this.positionService.position.X, photoStatus, photoLvl, data.ID));
        }
    }
}
