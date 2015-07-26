#region License 
//   Copyright 2015 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tainicom.VertexTypes
{
    public struct VertexPositionNormalColorTextureTangentBinormal : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;
        public Vector2 TextureCoordinate;
        public Vector3 Tangent;
        public Vector3 Binormal;

        #region IVertexType Members

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
                new VertexElement[] 
                {
                    new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                    new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement(40, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                    new VertexElement(52, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
                });
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        #endregion

        public VertexPositionNormalColorTextureTangentBinormal(Vector3 position, Vector3 normal, Vector3 tangent, Vector3 binormal, Color color, Vector2 textureCordinate)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
            this.Tangent = tangent;
            this.Binormal = binormal;
            this.TextureCoordinate = textureCordinate;
        }

    }
}
