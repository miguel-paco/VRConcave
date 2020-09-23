using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.Visualizers
{
    public class ServiceProvider : IServiceProvider
    {
        public List<IDialogTypeVisualizerService> services = new List<IDialogTypeVisualizerService>();
        public object GetService(Type serviceType)
        {
            foreach (object service in services)
            {
                return service;
            }
            return null;
        }
    }
}
