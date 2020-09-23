using FlyVRena2._0.Display;
using FlyVRena2._0.VirtualWorld.Subsystems;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.VirtualWorld.Services
{
    public abstract class DrawService
    {

        public IServiceProvider WO;
        public NameService nameService;
        public PositionService positionService;

        public Renderable rObj;
        public ShaderProgram sp;
        public Matrix4 _modelView;

        public DrawService(IServiceProvider WObj, VirtualWorld VW)
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

        public virtual void Render(ICamera camera) { }

        public virtual void Dispose()
        {
            rObj.Dispose();
            sp.Dispose();
        }
    }
}
