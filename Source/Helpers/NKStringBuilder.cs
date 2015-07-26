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

using System.Text;

namespace tainicom.Helpers
{
    class NKStringBuilder
    {
        private static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        
        /// <summary> 
        /// The actual string builder used by the string. We can get it and append directly if needed, but we can't reassign it or it'll mess up the pointer. 
        /// </summary> 
        public StringBuilder StringBuilder 
        { 
            get 
            { 
                return _stringBuilder; 
            } 
        } 
        private StringBuilder _stringBuilder; 
        private char[] numberBuffer = new char[25]; 
 
        /// <summary> 
        /// Declares a new No Trash String Builder. 
        /// </summary> 
        /// <param name="capacity">The starting and max capacity that the internal string builder should be set to. Only make it as big as you need.</param> 
        public NKStringBuilder(int capacity) 
        { 
            _stringBuilder = new StringBuilder(capacity, capacity);
        } 
 
        /// <summary> 
        /// Appends a number to the string without creating garbage. 
        /// </summary> 
        /// <param name="number">The number to append.</param> 
        public void AppendNumber(int number) 
        { 
            AppendNumber(number, 0); 
        } 
 
        /// <summary> 
        /// Appends a long number to the string without creating garbage. 
        /// </summary> 
        /// <param name="number">The long number to append.</param> 
        /// <param name="addCommas">Whether or not to add commas to the number.</param> 
        public void AppendNumber(long number, bool addCommas) 
        { 
            if (number < 0) 
            { 
                _stringBuilder.Append('-'); 
                number = -number; 
            } 
            int index = 0; 
            do 
            { 
                long digit = number % 10; 
                if ((index + 1) % 4 == 0) 
                { 
                    numberBuffer[index] = (char)(','); 
                    ++index; 
                }
                numberBuffer[index] = digits[digit]; 
                number /= 10; 
                ++index; 
            } while (number > 0); 
            for (--index; index >= 0; --index) 
            { 
                _stringBuilder.Append(numberBuffer[index]); 
            } 
        } 
 
        /// <summary> 
        /// Appends a 2 decimal floating point number without creating garbage. 
        /// </summary> 
        /// <param name="number">The float to append.</param> 
        public void AppendNumber(float number) 
        { 
            number *= 100f; 
            if (number < 0) 
            { 
                _stringBuilder.Append('-'); 
                number = -number; 
            } 
            float original = number; 
            int index = 0; 
            do 
            { 
                if (index == 2) 
                { 
                    numberBuffer[index] = (char)('.'); 
                    ++index; 
                } 
                int digit = (int)number % 10; 
                numberBuffer[index] = (char)('0' + digit); 
                number /= 10; 
                ++index; 
            } while (number > 0.99f); 
            if (original < 100) 
            { 
                if (original < 10) 
                { 
                    numberBuffer[index] = (char)('0'); 
                    ++index; 
                } 
                numberBuffer[index] = (char)('.'); 
                ++index; 
                numberBuffer[index] = (char)('0'); 
                ++index; 
            } 
            for (--index; index >= 0; --index) 
            { 
                _stringBuilder.Append(numberBuffer[index]); 
            } 
        } 
 
        /// <summary> 
        /// Appends a long number without creating garbage. 
        /// </summary> 
        /// <param name="number">The number to append.</param> 
        /// <param name="minDigits">The minimum number of digits, will add 0's to the front of the number.</param> 
        public void AppendNumber(long number, int minDigits) 
        { 
            if (number < 0) 
            { 
                _stringBuilder.Append('-'); 
                number = -number; 
            } 
            int index = 0; 
            do 
            { 
                long digit = number % 10;
                numberBuffer[index] = digits[digit]; 
                number /= 10; 
                ++index; 
            } while (number > 0 || index < minDigits); 
            for (--index; index >= 0; --index) 
            { 
                _stringBuilder.Append(numberBuffer[index]); 
            } 
        } 
 
        /// <summary> 
        /// Exposes the internal string builder's Append to make things a bit easier. 
        /// </summary> 
        /// <param name="Text"></param> 
        public void Append(string Text) 
        { 
            _stringBuilder.Append(Text); 
        } 
 
        public void Append(string text1, string text2) 
        { 
            _stringBuilder.Append(text1); 
            _stringBuilder.Append(text2); 
        } 
 
        public void Append(string text1, string text2, string text3) 
        { 
            _stringBuilder.Append(text1); 
            _stringBuilder.Append(text2); 
            _stringBuilder.Append(text3); 
        } 
 
        public void Append(string text1, string text2, string text3, string text4) 
        { 
            _stringBuilder.Append(text1); 
            _stringBuilder.Append(text2); 
            _stringBuilder.Append(text3); 
            _stringBuilder.Append(text4); 
        } 
 
        public void Append(string text1, string text2, string text3, string text4, string text5) 
        { 
            _stringBuilder.Append(text1); 
            _stringBuilder.Append(text2); 
            _stringBuilder.Append(text3); 
            _stringBuilder.Append(text4); 
            _stringBuilder.Append(text5); 
        } 
 
        /// <summary> 
        /// Clears out the string builder. 
        /// </summary> 
        public void Clear() 
        { 
            _stringBuilder.Remove(0, _stringBuilder.Length); 
        } 
 
        /// <summary> 
        /// Appends a number to the string without creating garbage, allows you to specify the minimum digits. 
        /// </summary> 
        /// <param name="number">The number to append.</param> 
        /// <param name="minDigits">The minimum number of digits.</param> 
        public void AppendNumber(int number, int minDigits) 
        { 
            if (number < 0) 
            { 
                _stringBuilder.Append('-'); 
                number = -number; 
            } 
            int index = 0; 
            do 
            { 
                int digit = number % 10;
                number = number / 10; 
                numberBuffer[index++] = digits[digit];
            } while(number > 0);

            while (index < minDigits) numberBuffer[index++] = ' ';

            for (--index; index >= 0; --index) 
            {	
                _stringBuilder.Append(numberBuffer[index]); 
            } 
        } 
    } 
}
