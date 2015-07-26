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

namespace tainicom.Maths
{
    public class TncMathHelper
    {
        /// <summary>
        /// Wraps the specified value into a range.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>Result of the wrapping.</returns>
        public static int WrapValue(int value, int min, int max)
        {
            if (min == max) return min;
            int v = (((value - min) % (max - min)));
            if (value > max)
            {
                return min + v;
            }
            if (value < min) return max + v;

            return value;
        }
        
        /// <summary>
        /// Wraps the specified value into a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>Result of the wrapping.</returns>
        public static float WrapValue(float value, float min, float max)
        {
            if (min > max)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            value -= min;

            float rangeSize = max - min;
            if (rangeSize == 0.0f) return max;

            return (float)(value - (rangeSize * Math.Floor(value / rangeSize)) + min);
        }

    }
}
