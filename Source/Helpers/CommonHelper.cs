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

namespace tainicom.Helpers
{
    public class CommonHelper
    {
        public Random Random;

         #region implement Sigleton
        static CommonHelper _instance;
        public static CommonHelper Current {get { return (_instance == null) ? new CommonHelper() : _instance; } }
        #endregion 

        private CommonHelper()
        {
            Random = new Random((int)DateTime.Now.Ticks);
            CommonHelper._instance = this;
        }

        //static classes for bound cheking,parameter checking, Crossplaform P-Trace, etc
#if DEBUG
        public static void Trace(string message)
        {
            #if (NETFX_CORE || PHONE)
                System.Diagnostics.Debug.WriteLine(message);
            #elif WINDOWS
                System.Diagnostics.Trace.WriteLine(message);
            #endif
        }
        public static void Trace(StringBuilder message)
        {
            Trace(message.ToString());
        }
        public static void Trace(Object message)
        {
            Trace(message.ToString());
        }
        
        
        public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);
            if (argumentValue.Length == 0) throw new ArgumentException("String cannot be empty.", argumentName);
            return;
        }

        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
            return;
        }
#endif
        public static void ShowDialog(string message)
        {
            #if PHONE
            System.Windows.MessageBox.Show(message);
            #elif NETFX_CORE
        
            #endif
        }


    }
}
