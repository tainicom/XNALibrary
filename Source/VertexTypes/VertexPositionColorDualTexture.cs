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
    public struct VertexPositionColorDualTexture : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoordinate0;
        public Vector2 TextureCoordinate1;

        #region IVertexType Members

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
                new VertexElement[] 
                {
                    new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                    new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
                });

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
        #endregion

        public VertexPositionColorDualTexture(Vector3 position, Color color, Vector2 textureCoordinate0,Vector2 TextureCoordinate1)
        {
            this.Position = position;
            this.Color = color;
            this.TextureCoordinate0 = textureCoordinate0;
            this.TextureCoordinate1 = TextureCoordinate1;
        }

    }
}
