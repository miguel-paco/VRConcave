using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.Display
{
    public class VertexFactory
    {
        public static ColoredVertex[] CreateSolidSquare(float side, float z, Color4 color)
        {
            side = side / 2f;
            ColoredVertex[] vertices =
            {
                new ColoredVertex(new Vector4(-side, -side, z, 1.0f),    color),
                new ColoredVertex(new Vector4(side, -side, z, 1.0f),     color),
                new ColoredVertex(new Vector4(-side, side, z, 1.0f),     color),
                new ColoredVertex(new Vector4(-side, side, z, 1.0f),     color),
                new ColoredVertex(new Vector4(side, -side, z, 1.0f),     color),
                new ColoredVertex(new Vector4(side, side, z, 1.0f),      color),
            };
            return vertices;
        }

        public static ColoredVertex[] CreateSolidCircle(float r, int np, float[] center, float z, Color4 color)
        {
            ColoredVertex[] vertices = new ColoredVertex[np+1];
            int aux = 0;
            for (float ang = 0.0f; ang < 2* 3.1415f; ang += (2 * 3.1415f / np))
            {
                vertices[aux] = new ColoredVertex(new Vector4(center[0]+(float)Math.Sin(ang)*r+center[1], center[1] + (float)Math.Cos(ang) * r - center[0], z, 1.0f), color);
                aux = aux + 1;
                //vertices[aux] = new ColoredVertex(new Vector4(center[0] + (float)Math.Sin(ang+0.1) * r + center[1], center[1] + (float)Math.Cos(ang+0.1) * r - center[0], z, 1.0f), color);
                //aux = aux + 1;
            }
            return vertices;
        }

        public static ColoredVertex[] CreateSolidEllipse(float rx, float ry, int np, float[] center, float z, Color4 color)
        {
            ColoredVertex[] vertices = new ColoredVertex[np + 1];
            int aux = 0;


            for (float ang = 0.0f; ang < 2 * 3.1415f; ang += (2 * 3.1415f / np))
            {
                vertices[aux] = new ColoredVertex(new Vector4(center[0] + (float)Math.Sin(ang) * rx + center[1], center[1] + (float)Math.Cos(ang) * ry - center[0], z, 1.0f), color);
                aux = aux + 1;
                //vertices[aux] = new ColoredVertex(new Vector4(center[0] + (float)Math.Sin(ang+0.1) * r + center[1], center[1] + (float)Math.Cos(ang+0.1) * r - center[0], z, 1.0f), color);
                //aux = aux + 1;
            }
            return vertices;
        }

        public static TexturedVertex[] CreateTexturedSquare(float side, float z, float textureWidth, float textureHeight)
        {
            float h = textureHeight;
            float w = textureWidth;
            side = side / 2f;
            TexturedVertex[] vertices =
            {
                new TexturedVertex(new Vector4(-side, -side, z, 1.0f),    new Vector2(0, h)),
                new TexturedVertex(new Vector4(side, -side, z, 1.0f),     new Vector2(w, h)),
                new TexturedVertex(new Vector4(-side, side, z, 1.0f),     new Vector2(0, 0)),
                new TexturedVertex(new Vector4(-side, side, z, 1.0f),     new Vector2(0, 0)),
                new TexturedVertex(new Vector4(side, -side, z, 1.0f),     new Vector2(w, h)),
                new TexturedVertex(new Vector4(side, side, z, 1.0f),      new Vector2(w, 0)),
            };
            return vertices;
        }

    }
}
