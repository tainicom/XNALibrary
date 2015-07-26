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
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace tainicom.Tweens
{
    public class Scheduler : Timer
    {
        List<SchedulerEvent> eventList = new List<SchedulerEvent>(5);
        int _spanId;
        float _delta;
        bool _eventReached;

        public int SpanId { get { return _spanId; } }
        public float Delta { get { return _delta; } }
        public bool EventReached { get { return _eventReached; } }
        public List<SchedulerEvent> Events { get { return eventList; } }
        public string Name { get { return Events[SpanId].Name; } }
        
        public Scheduler():base()
        {
        }

        public override void Reset()
        {            
            base.Reset();
            eventList.Clear();
            eventList.Add(new SchedulerEvent("time zero",TimeSpan.Zero));
            this._spanId = 0;
            this._delta = 0f;
            this._eventReached = false;
        }


        public void AddEventSpan(float seconds)
        {
            AddEventSpan(String.Empty, seconds);
        }
        public void AddEventSpan(string name, float seconds)
        {
            TimeSpan timeEvent = eventList[eventList.Count - 1].Time + TimeSpan.FromSeconds(seconds);
            eventList.Add(new SchedulerEvent(name,timeEvent));
        }
        
        public void DriftToEvent(int id)
        {
            base.elapsedTime = eventList[id].Time;
            _spanId = id;
            _eventReached = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;
            base.Update(gameTime);

            Next();
            // stop , if we reached the last event
            if ( _spanId >= (eventList.Count - 1)) return;

            TimeSpan aEvent = eventList[_spanId].Time;
            TimeSpan bEvent = eventList[_spanId + 1].Time;

            //check if we reach next event
            if (base.elapsedTime >= bEvent)
            {
                _eventReached = true;
                _delta = 1f;
                return;
            }

            // calculate delta for reaching next event
            TimeSpan dt = base.elapsedTime - aEvent;
            TimeSpan ds = bEvent - aEvent;
            _delta = (float)(dt.TotalMilliseconds / ds.TotalMilliseconds);
            return;
        }

        internal void Next()
        {
            if (_eventReached && _spanId < (eventList.Count-1))
            {
                _eventReached = false;
                _spanId++;
                _delta = 0f;
            }
            return;
        }

        internal float Lerp(float from, float to)
        {
            //return MathHelper.Lerp(from, to, _delta);
            return from + (to - from) * _delta;
        }
        
    }

    public struct SchedulerEvent
    {
        public readonly String Name;
        public readonly TimeSpan Time;
        public SchedulerEvent(String name, TimeSpan time)
        {
            this.Name = name;
            this.Time = time;
        }
    }

}
