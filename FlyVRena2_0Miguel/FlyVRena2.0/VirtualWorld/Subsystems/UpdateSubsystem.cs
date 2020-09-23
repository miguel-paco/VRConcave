using FlyVRena2._0.ImageProcessing;
using FlyVRena2._0.VirtualWorld.Services;
using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Subsystems
{
    public class UpdateSubsystem
    {
        public UpdateSubsystem(VirtualWorld VW) {}

        // List of objects registered to draw and a camera (only one camera should be attributed at a time)
        public Dictionary<string, UpdateService<FilteredData>> UpdateServices = new Dictionary<string, UpdateService<FilteredData>>();

        public void Update(double time)
        {
            foreach (UpdateService<FilteredData> ds in UpdateServices.Values)
            {
                ds.Update(time);
            }
        }

        public void OnExit()
        {
            foreach (UpdateService<FilteredData> ds in UpdateServices.Values)
            {
                ds.IsExiting();
            }
        }

        public void Dispose()
        {
            foreach (UpdateService<FilteredData> ds in UpdateServices.Values)
            {
                ds.Dispose();
            }
        }
    }
}
