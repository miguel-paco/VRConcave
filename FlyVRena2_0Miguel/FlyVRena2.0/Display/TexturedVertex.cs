using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.Display
{
    public struct TexturedVertex
    {
        public const int Size = (4 + 2) * 4; // size of struct in bytes

        private readonly Vector4 _position;
        private readonly Vector2 _textureCoordinate;

        public TexturedVertex(Vector4 position, Vector2 textureCoordinate)
        {
            _position = position;
            _textureCoordinate = textureCoordinate;
        }
    }
}
