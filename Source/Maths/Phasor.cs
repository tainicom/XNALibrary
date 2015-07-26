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
    //a phasor is A * cos(ùt+ö)
    public struct Phasor
    {

        public static float HalfCircle = 3.14159265f;
        public static float FullCicle = 2 * HalfCircle;

        public readonly float A;
        public readonly float Frequency;
        public readonly float Period;
        public readonly float AngularFrequency; // ù (Angular frequency)
        public readonly float Phase; // ö

        public Phasor(float frequency, float A, float phase)
        {
            this.A = A;
            this.Frequency = frequency;
            this.Period = 1 / Frequency;
            this.Phase = phase;
            this.AngularFrequency = FullCicle * Frequency;
        }

        public static Phasor CreateFromFreq(float freq)
        {
            return new Phasor(freq, 1f, 0f);
        }

        public static Phasor CreateFromPeriod(float period)
        {
            return new Phasor(1/period, 1f, 0f);
        }

		public float Value(Microsoft.Xna.Framework.GameTime gameTime)
        {
            return Value(gameTime.TotalGameTime.TotalSeconds);
        }

        public float Value(TimeSpan timeSpan)
        {
            return Value(timeSpan.TotalSeconds);
        }

        public float Value(double seconds)
        {
            float value = A * (float)Math.Sin((double)(seconds * AngularFrequency + Phase));
            return value;
        }

        public float ValueRandians(Microsoft.Xna.Framework.GameTime gameTime)
        {
            return ValueRandians(gameTime.TotalGameTime.TotalSeconds+Phase);
        }

        public float ValueRandians(double seconds)
        {
            return (float)seconds * Frequency % FullCicle;
        }

        public float ValueLinear(double seconds)
        {
            return (float)seconds * Frequency % 1f;
        }

    }
}
