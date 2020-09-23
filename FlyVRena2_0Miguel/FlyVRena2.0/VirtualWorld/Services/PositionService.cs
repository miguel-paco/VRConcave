using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services
{
    public class PositionService
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 velocity;
        public Vector3 rotationVelocity;
        public float scale;

        public PositionService(IServiceProvider WObj, VirtualWorld VW, Vector3 position, Vector3 rotation, float scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            velocity = new Vector3(0, 0, 0);
            rotationVelocity = new Vector3(0, 0, 0);
        }
        public PositionService()
        {
            position = new Vector3(0, 0, 0);
            rotation = new Vector3(0, 0, 0);
            velocity = new Vector3(0, 0, 0);
            rotationVelocity = new Vector3(0, 0, 0);
            scale = 1;
        }
        /* Method that returns the world matrix */
        public Matrix4 PositionMatrix()
        {
            return Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(position);
        }
    }
}
