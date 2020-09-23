using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services
{
    public interface ICamera
    {
        Matrix4 LookAtMatrix { get; }
        Matrix4 ProjectionMatrix { get; }
        void Update(double time, double delta);
    }
}
