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

using Android.Content;
using Android.Views;
using Microsoft.Xna.Framework;
using System;

namespace tainicom.Helpers
{
    class AndroidOrientationEventListener : OrientationEventListener
    {
        DisplayOrientation _displayOrientation = DisplayOrientation.Unknown;
        
        internal int Angle { get; private set; }
        internal DisplayOrientation DisplayOrientation { get { return _displayOrientation; } }
        internal TimeSpan Delay { get; set; }

        TimeSpan _timer = TimeSpan.Zero;
        DisplayOrientation _currentDisplayOrientation = DisplayOrientation.Unknown;
        
        public AndroidOrientationEventListener(Context context):base(context)
		{
            Angle = -1;
            Delay = TimeSpan.FromSeconds(1);
		}

        internal void Update(GameTime gameTime)
        {
            if (_timer > TimeSpan.Zero)
            {
                _timer -= gameTime.ElapsedGameTime;
                if(_timer < TimeSpan.Zero)
                    _timer = TimeSpan.Zero;
            }
            
            if (_timer == TimeSpan.Zero)
            {
                if (_displayOrientation == _currentDisplayOrientation) return;
                _displayOrientation = _currentDisplayOrientation;
                OnDisplayOrientationChanged(EventArgs.Empty);
            }
            return;                        
        }

        public override void OnOrientationChanged(int orientation)
        {
            this.Angle = orientation;
            DisplayOrientation displayOrientation = GetDisplayOrientation(Angle);
            if (_currentDisplayOrientation == displayOrientation) return;

            _currentDisplayOrientation = displayOrientation;
            _timer = Delay;
        }

        internal static DisplayOrientation GetDisplayOrientation(int angle)
        {
            if (45 <= angle && angle < 90 + 45)
                return DisplayOrientation.LandscapeLeft;
            else if (180 + 45 <= angle && angle < 180 + 45 + 90)
                return DisplayOrientation.LandscapeRight;
            else if (90 + 45 <= angle && angle < 90 + 45 + 90)
                return DisplayOrientation.PortraitDown;
            else
                return DisplayOrientation.Portrait;
        }

        private readonly object DisplayOrientationChangedEventLock = new object();
        private EventHandler<EventArgs> DisplayOrientationChangedEvent;        
        /// <summary>
        /// Event raised after the <see cref="Text" /> property value has changed.
        /// </summary>
        public event EventHandler<EventArgs> DisplayOrientationChanged
        {
            add { lock (DisplayOrientationChangedEventLock) { DisplayOrientationChangedEvent += value; } }
            remove { lock (DisplayOrientationChangedEventLock) { DisplayOrientationChangedEvent -= value; } }
        }
        /// <summary>
        /// Raises the <see cref="DisplayOrientationChanged" /> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs" /> object that provides the arguments for the event.</param>
        protected virtual void OnDisplayOrientationChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = null;
            lock (DisplayOrientationChangedEventLock)
            {
                handler = DisplayOrientationChangedEvent;
                if (handler == null) return;
            }
            handler(this, e);
        }

    }
}