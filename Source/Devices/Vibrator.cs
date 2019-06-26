#region License 
//   Copyright 2015-2018 Kastellanos Nikolaos
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
using System;

namespace tainicom.Devices
{
    public class Vibrator: IGameComponent, IUpdateable
    {
        bool _enabled;
        int  _updateOrder;
        float _power;
        float _dumping;
        float _masterPower;
        float _startingThreashold;

        // native device
#if ANDROID
        Android.OS.Vibrator _vibrator;
        private bool _hasVibrator = true;
#endif

        public float Dumping
        {
            get { return _dumping; }
            set { _dumping = value; }
        }
        public float MasterPower
        {
            get { return _masterPower; }
            set { _masterPower = value; }
        }

        /// <summary>
        /// Usually a motor won't start for PWM signals below ~20%.
        /// The final output is mapped to the range [0 - 1] --> [StartingThreashold - 1]
        /// A typical value for StartingThreashold is between 0.10f and 0.25f.
        /// </summary>
        public float StartingThreashold
        {
            get { return _startingThreashold; }
            set { _startingThreashold = value; }
        }

        private static Vibrator _instance;
        public static Vibrator Current 
        {
            get
            {
                if (_instance == null) _instance = new Vibrator();
                return _instance;
            }
        }

        private Vibrator()
        {
            _enabled = true;
            _updateOrder = int.MaxValue;
            _power = 0;
            _dumping = 1.0f;
            _masterPower = 1;
            _startingThreashold = 0.0f;

            // init device
            #if ANDROID
            _vibrator = (Android.OS.Vibrator)Android.App.Application.Context.GetSystemService(Android.Content.Context.VibratorService);
            try
            {
                _hasVibrator = _vibrator.HasVibrator;
            }
            catch { /* ignore */ }
            #endif
        }
        
        public void Vibe(float power)
        {
            _power = MathHelper.Max(_power, power);
            return;
        }


        #region IGameComponent Members        
        void IGameComponent.Initialize() { }
        #endregion


        #region IUpdateable Members

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                OnEnabledChanged(EventArgs.Empty);
            }
        }
        
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder == value) return;
                _updateOrder = value;
                OnUpdateOrderChanged(EventArgs.Empty);
            }
        }
        
        public void Update(GameTime gameTime)
        {
            if (_power < 0.02f)
            {
                _power = 0;
                return;
            }
            
            float power = _power * _masterPower;
            if (power < 0.02f) 
                return;

            // map power value [0 - 1] -> [minPower - 1]
            power = _startingThreashold + power * (1f-_startingThreashold);

            double elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
            double dutyCycle = power * elapsedTime;
            
            // limit power to 100% PWM.
            dutyCycle = Math.Min(dutyCycle,elapsedTime);


            #if WP7 || WP8
            Microsoft.Devices.VibrateController.Default.Start(TimeSpan.FromSeconds(dutyCycle));
            #elif WP8_1
            Windows.Phone.Devices.Notification.VibrationDevice.GetDefault().Vibrate(TimeSpan.FromSeconds(dutyCycle));
            #elif ANDROID
            try
            {
                if(_hasVibrator)
                {
                    long ms = (long)(dutyCycle*1000);
                    _vibrator.Vibrate(ms);
                }
            }
            catch { /* ignore */ }
            #endif
            
            // dump power
            _power *= (1.0f - _dumping);
            return;
        }
        
        private EventHandler<EventArgs> enabledChangedEvent;
        private EventHandler<EventArgs> updateOrderChangedEvent;

        event EventHandler<EventArgs> IUpdateable.EnabledChanged
        {
            add { enabledChangedEvent += value; }
            remove { enabledChangedEvent -= value; }
        }       
        event EventHandler<EventArgs> IUpdateable.UpdateOrderChanged
        {
            add { updateOrderChangedEvent += value; }
            remove { updateOrderChangedEvent -= value; }
        }

        protected virtual void OnEnabledChanged(EventArgs e)
        {
            var handler = enabledChangedEvent;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnUpdateOrderChanged(EventArgs e)
        {
            var handler = updateOrderChangedEvent;
            if (handler != null) handler(this, e);
        }
        #endregion


        public static void Vibrate(TimeSpan duration)
        {
            #if WP7 || WP8
            Microsoft.Devices.VibrateController.Default.Start(duration);
            #elif WP8_1
            Windows.Phone.Devices.Notification.VibrationDevice.GetDefault().Vibrate(duration);
            #elif ANDROID
            try
            {
                var vibrator = Vibrator.Current;
                if (vibrator._hasVibrator)
                {
                    vibrator._vibrator.Vibrate((long)duration.TotalMilliseconds);
                }
            }
            catch { /* ignore */ }
            #endif
        }
    }
}
