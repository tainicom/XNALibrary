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
    

    public class MarketplaceHelper
    {
        private bool _isTrial;
        public bool IsTrial
        {
            get { return (EmulateTrial.HasValue) ? EmulateTrial.Value : _isTrial; }
        }
        public bool IsSigned { get; private set; }
        public Nullable<bool> EmulateTrial { get; set; }

        #region implement Sigleton
        static MarketplaceHelper _instance;
        public static MarketplaceHelper Current 
        { 
            get
            { 
                if (_instance==null)_instance=new MarketplaceHelper();
                return _instance;
            } 
        }
        #endregion 

        private MarketplaceHelper()
        {
            UpdateIsTrial();
            UpdateIsSigned();
        }

        public void UpdateIsTrial()
        {
            _isTrial = false;
            #if PHONE
            _isTrial = (new Microsoft.Phone.Marketplace.LicenseInformation()).IsTrial();
            #elif WINDOWS_UAP
            _isTrial = true;
            #elif NETFX_CORE
            _isTrial = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.IsTrial;
            #endif
        }

        public void UpdateIsSigned()
        {
            IsSigned = true;
            #if PHONE
            //try
            //{ 
            //IsSigned = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication().FileExists("WMAppPRHeader.xml");            
            //} catch (Exception ex) {}
            #endif
            return;
        }
        

    }
}
