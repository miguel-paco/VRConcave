using System;
using OpenTK.Graphics.OpenGL4;

namespace FlyVRena2._0.Display
{
    public class ColoredObject : Renderable
    {
        public ColoredObject(ColoredVertex[] vertices, int program)
            : base(program, vertices.Length)
        {
            // create first buffer: vertex
            GL.NamedBufferStorage(Buffer, ColoredVertex.Size * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);
            GL.VertexArrayAttribBinding(VertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 0);
            GL.VertexArrayAttribFormat(VertexArray, 0, 4, VertexAttribType.Float, false, 0);
            GL.VertexArrayAttribBinding(VertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 1);
            GL.VertexArrayAttribFormat(VertexArray, 1, 4, VertexAttribType.Float, false, 16);

            // link the vertex array and buffer and provide the stride as size of Vertex
            GL.VertexArrayVertexBuffer(VertexArray, 0, Buffer, IntPtr.Zero, ColoredVertex.Size);
        }
    }
}
