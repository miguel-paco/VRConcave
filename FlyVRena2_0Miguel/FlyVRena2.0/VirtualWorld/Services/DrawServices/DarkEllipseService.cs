using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyVRena2._0.Display;
using FlyVRena2._0.VirtualWorld.Subsystems;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace FlyVRena2._0.VirtualWorld.Services.DrawServices
{
    public class DarkEllipseService : DrawService
    {
        public int np;
        public DarkEllipseService(IServiceContainer wObj, VirtualWorld VW, int circleRadiusX, int circleRadiusY, int np) : base(wObj, VW)
        {
            this.np = np;
            if (VW.render != null && circleRadiusX != 0)
            {
                VW.render.ShaderProgramList.TryGetValue("solidProgram", out sp);
                rObj = new ColoredObject(VertexFactory.CreateSolidEllipse(circleRadiusX, circleRadiusY, np, new float[] { 0, 0 }, 1, Color4.Black), sp.Id);
                VW.render.DrawServices.Add(nameService.name, this);
            }
        }


        public override void Render(ICamera camera)
        {
            rObj.Bind();
            GL.Enable(EnableCap.Blend);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, np);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _modelView = positionService.PositionMatrix() * camera.LookAtMatrix;
            GL.UniformMatrix4(21, false, ref _modelView);
            rObj.Render();
            base.Render(camera);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
