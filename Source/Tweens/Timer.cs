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

using System;
using Microsoft.Xna.Framework;

namespace tainicom.Tweens
{
    public class Timer: IGameComponent, IUpdateable
    {
        bool _enabled;
        int  _updateOrder;
        protected TimeSpan elapsedTime;

        public TimeSpan ElapsedTime {get {return elapsedTime;}}

        public Timer()
        {
            _enabled = true;
            _updateOrder = int.MinValue;

            Reset();
        }

        public virtual void Reset()
        {
            elapsedTime = TimeSpan.Zero;
        }


        #region IGameComponent Members        
        void IGameComponent.Initialize() { }
        #endregion


        #region implement IUpdateable
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


        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled) return;
            elapsedTime += gameTime.ElapsedGameTime;
        }
        #endregion
    }
}
