using FlyVRena2._0.Display;
using FlyVRena2._0.VirtualWorld.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace FlyVRena2._0.VirtualWorld.Subsystems
{
    public class RenderSubsystem
    {
        public Dictionary<string, ShaderProgram> ShaderProgramList;
        public ICamera camera;
        public int Size;
        public RenderSubsystem(VirtualWorld VW, ICamera camera, int size)
        {
            this.camera = camera;
            this.Size = size;
            ShaderProgramList = new Dictionary<string, ShaderProgram>();
            LoadShaderPrograms();
        }

        private void LoadShaderPrograms()
        {
            var textureProgram = new ShaderProgram();
            textureProgram.AddShader(ShaderType.VertexShader, @"..\..\..\Shaders\Texture\simplePipeTexVert.c");
            textureProgram.AddShader(ShaderType.FragmentShader, @"..\..\..\Shaders\Texture\simplePipeTexFrag.c");
            textureProgram.Link();
            ShaderProgramList.Add("textureProgram", textureProgram);

            var solidProgram = new ShaderProgram();
            solidProgram.AddShader(ShaderType.VertexShader, @"..\..\..\Shaders\SolidColor\simplePipeVert.c");
            solidProgram.AddShader(ShaderType.FragmentShader, @"..\..\..\Shaders\SolidColor\simplePipeFrag.c");
            solidProgram.Link();
            ShaderProgramList.Add("solidProgram", solidProgram);
        }

        // List of services registered to draw and a camera (only one camera should be attributed at a time)
        public Dictionary<string, DrawService> DrawServices = new Dictionary<string, DrawService>();

        //public static readonly object obj = new object();
        public void Draw(double time)
        {
            int lastProgram = -1;
            foreach (DrawService ds in DrawServices.Values)
            {
                var program = ds.rObj.Program;
                if (lastProgram != program)
                {
                    var _pm = camera.ProjectionMatrix;
                    GL.UniformMatrix4(20, false, ref _pm);
                }
                lastProgram = ds.rObj.Program;
                ds.Render(camera);
            }
        }

        public void OnExit()
        {
            foreach (ShaderProgram sp in ShaderProgramList.Values)
            {
                sp.Dispose();
            }
        }
    }
}
