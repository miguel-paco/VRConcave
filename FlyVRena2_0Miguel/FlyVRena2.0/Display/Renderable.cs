﻿using System;
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
        }
        public virtual void Bind()
        {
            GL.UseProgram(Program);
            GL.BindVertexArray(VertexArray);
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
