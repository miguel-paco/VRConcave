using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace FlyVRena2._0.Display
{
    public abstract class Renderable : IDisposable
    {
        public readonly int Program;
        protected readonly int VertexArray;
        protected readonly int Buffer;
        protected readonly int VerticeCount;

        protected Renderable(int program, int vertexCount)
        {
            Program = program;
            VerticeCount = vertexCount;
            VertexArray = GL.GenVertexArray();
            Buffer = GL.GenBuffer();

            GL.BindVertexArray(VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer);
    //        GL.UseProgram(Program);
        }
        public virtual void Bind()
        {
            GL.UseProgram(Program);
            // Program = 1 makes textures and has no corrections, Program = 2 makes shaders and needs the DUMMY object and the -1 Correction described bellow
            if (Program == 1) { GL.BindVertexArray(VertexArray); }
            if (Program == 2) { GL.BindVertexArray(VertexArray-1); } // -1 toguether with the DUMMY object makes it so that the objects drawn are correctly attributed. This is a Patch Fix for the interactions with the GL interface
        }
        public virtual void Render()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, VerticeCount);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GL.DeleteVertexArray(VertexArray);
                GL.DeleteBuffer(Buffer);
            }
        }
    }
}
