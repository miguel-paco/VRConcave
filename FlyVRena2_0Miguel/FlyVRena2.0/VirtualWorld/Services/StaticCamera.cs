using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services
{
    public class StaticCamera : ICamera
    {
        public Matrix4 LookAtMatrix { get; }
        public Matrix4 ProjectionMatrix { get; }
        public StaticCamera(int size)
        {
            Vector3 position;
            position.X = 0;
            position.Y = 0;
            position.Z = 0;
            LookAtMatrix = Matrix4.LookAt(position, Vector3.UnitZ, Vector3.UnitY);
            ProjectionMatrix = Matrix4.CreateOrthographic(size, size, 0.01f, 1000);
        }

        public void Update(double time, double delta)
        { }
    }
}
