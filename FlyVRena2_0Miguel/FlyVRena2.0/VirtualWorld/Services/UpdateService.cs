using FlyVRena2._0.ImageProcessing;
using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlyVRena2._0.VirtualWorld.Services
{
    public abstract class UpdateService<T> : Module<T> where T : FilteredData
    {
        public IServiceProvider WO;
        public PositionService positionService;
        public NameService nameService;
        public UpdateService(IServiceProvider WObj, VirtualWorld VW)
        {
            WO = WObj;
            if (WObj.GetService(typeof(NameService)) != null)
            {
                nameService = (NameService)WObj.GetService(typeof(NameService));
            }
            if (WObj.GetService(typeof(PositionService)) != null)
            {
                positionService = (PositionService)WObj.GetService(typeof(PositionService));
            }
        }

        /* Update routine */
        public virtual void Update(double time) { }

        /* Routine to Exit safely from the Update (for example finish filestreams) */
        public virtual void IsExiting()
        {
            this.Abort();
        }

        public void DisposeModule()
        {
            this.Dispose();
        }

    }

    [Serializable]
    [XmlRoot("Trial")]
    public class Trial
    {
        [XmlAttribute("TrialType")]
        public string TrialType;
        [XmlAttribute("TrialDuration")]
        public double TrialDuration;
    }
}
