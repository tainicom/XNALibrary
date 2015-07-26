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

using System;
using Microsoft.Xna.Framework;

namespace tainicom.Maths
{
    public class MathHelper2D
    {
        public static Vector2[] FindTangentLines(Vector2 c1, float r1, Vector2 c2, float r2)
        {
            Vector2 l1a, l1b; //line 1
            Vector2 l2a, l2b; //line 2
            l1a = l1b = l2a = l2b = Vector2.Zero;

            //make circle 1 the bigger
            if (r1 < r2)
            {
                Vector2 ctmp = c1;
                c1 = c2; c2 = ctmp;
                float rtmp = r1;
                r1 = r2; r2 = rtmp;
            }

            //find distance
            Vector2 dir = c2 - c1;
            float d = dir.Length();
            //new circle
            Vector2 c3 = c1;
            float r3 = r1 - r2;
            //distance of Tangent line
            double h = Math.Sqrt(d * d - r3 * r3);
            double y = Math.Sqrt(h * h + r2 * r2);
            double theta = Math.Acos((r1 * r1 + d * d - y * y) / (2 * r1 * d));
            theta += Math.Atan2(dir.Y, dir.X);
            //calc points for line 1
            l1a.X = c1.X + r1 * (float)Math.Cos(theta);
            l1a.Y = c1.Y + r1 * (float)Math.Sin(theta);
            l1b.X = c2.X + r2 * (float)Math.Cos(theta);
            l1b.Y = c2.Y + r2 * (float)Math.Sin(theta);

            //calc points for line 2
            l2a.X = c1.X + r1 * (float)Math.Cos(-theta);
            l2a.Y = c1.Y + r1 * (float)Math.Sin(-theta);
            l2b.X = c2.X + r2 * (float)Math.Cos(-theta);
            l2b.Y = c2.Y + r2 * (float)Math.Sin(-theta);

            return new Vector2[4] { l1a, l1b, l2a, l2b };
        }

    }
}
