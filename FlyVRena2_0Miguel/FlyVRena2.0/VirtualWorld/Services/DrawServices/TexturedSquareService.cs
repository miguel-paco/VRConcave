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
    public class TexturedSquareService : DrawService
    {
        public TexturedSquareService(IServiceContainer wObj, VirtualWorld VW, string texturePath, int textureSize) : base(wObj, VW)
        {
            if (VW.render != null && texturePath != "")
            {
                VW.render.ShaderProgramList.TryGetValue("textureProgram", out sp);
                rObj = new TexturedObject(VertexFactory.CreateTexturedSquare(textureSize, 1, textureSize, textureSize), sp.Id, texturePath);
                VW.render.DrawServices.Add(nameService.name, this);
            }
        }

        public override void Render(ICamera camera)
        {
            rObj.Bind();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _modelView = positionService.PositionMatrix() * camera.LookAtMatrix;
            GL.UniformMatrix4(21, false, ref _modelView);
            rObj.Render();
            base.Render(camera);
        }

        public override void Dispose()
        { }
    }
}
