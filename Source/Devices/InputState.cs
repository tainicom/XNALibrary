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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;


namespace tainicom.Devices
{
    public class InputState
    {
        #region Fields
        // Touch
        private TouchLocation _mouseTouchLocation;
        private TouchLocation _prevMouseTouchLocation;
        public bool IsGestureAvailable;
        private TouchCollection _touchCollection;
        private List<GestureSample> _gestures = new List<GestureSample>(4);

        // GamePad (... and Back button on phones)
        private GamePadState _prevGamePadState;
        private GamePadState _gamePadState;
        
        // Keyboard
        private KeyboardState _prevKeyboardState;
        private KeyboardState _keyboardState;

        // Mouse
        private MouseState _prevMouseState;
        private MouseState _mouseState;
        #endregion

        /// <summary>
        /// Emulate touch input with mouse
        /// </summary>
        public bool EmulateTouch { get;set; }
        private DateTime _pressTimestamp;

        #region Properties
        public TouchCollection TouchCollection { get { return _touchCollection; } }
        public List<GestureSample> Gestures { get { return _gestures; } }
        public GamePadState GamePadState { get { return _gamePadState; } }
        public GamePadState PrevGamePadState { get { return _prevGamePadState; } }
        public KeyboardState KeyboardState { get { return _keyboardState; } }
        public KeyboardState PrevKeyboardState { get { return _prevKeyboardState; } }
        public MouseState MouseState { get { return _mouseState; } }
        public MouseState PrevMouseState { get { return _prevMouseState; } }
        

        #endregion

        #region Initialization
        public InputState()
        {
            EmulateTouch = true;
        }
        #endregion


        /// <param name="isActive">The value of Game.IsActive</param>
        public void Update(bool isActive)
        {
            _prevGamePadState = _gamePadState;
            _gamePadState = GamePad.GetState(PlayerIndex.One);

            _prevKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            _touchCollection = TouchPanel.GetState();

            IsGestureAvailable = false;
            if(TouchPanel.EnabledGestures != GestureType.None)
            {
                IsGestureAvailable = TouchPanel.IsGestureAvailable;
                _gestures.Clear();
                while (TouchPanel.IsGestureAvailable)
                {
                    _gestures.Add(TouchPanel.ReadGesture());
                }
            }

#if (!WP7) // Ignore mouse state from Windows Phone 7. It will emulate Mouse from Primary Touch location.
            _prevMouseState = _mouseState;
            _mouseState = Mouse.GetState();
#endif
            if (EmulateTouch)
            {
                EmulateStateWithMouse(isActive);
                EmulateGesturesWithMouse(isActive);
            }
            return;
        }

        private void EmulateStateWithMouse(bool isActive)
        {
            Vector2 position = new Vector2(_mouseState.X, _mouseState.Y);
            _prevMouseTouchLocation = _mouseTouchLocation;

            if (isActive )
            {
                //pressed
                if (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
                {
                    _mouseTouchLocation = new TouchLocation(10000, TouchLocationState.Pressed, position);
                }
                //moved
                if (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Pressed)
                {
                    _mouseTouchLocation = new TouchLocation(_mouseTouchLocation.Id, TouchLocationState.Moved, position, _mouseTouchLocation.State, _mouseTouchLocation.Position);
                }
                //
                if (_mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed)
                {
                    _mouseTouchLocation = new TouchLocation(_mouseTouchLocation.Id, TouchLocationState.Released, position, _mouseTouchLocation.State, _mouseTouchLocation.Position);
                }
                if (_mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Released)
                {
                    _mouseTouchLocation = new TouchLocation();
                }
            }
            else
            {
                if (_prevMouseState.LeftButton == ButtonState.Pressed)
                {
                    _mouseTouchLocation = new TouchLocation(_mouseTouchLocation.Id, TouchLocationState.Released, position, _mouseTouchLocation.State, _mouseTouchLocation.Position);
                }
            }

            //replace touchCollection 
            if (_mouseTouchLocation.State != TouchLocationState.Invalid)
            {
                TouchLocation[] touchLocationArray = new TouchLocation[_touchCollection.Count + 1];
                _touchCollection.CopyTo(touchLocationArray, 0);
                touchLocationArray[touchLocationArray.Length - 1] = _mouseTouchLocation;
                _touchCollection = new TouchCollection(touchLocationArray);
            }
            return;
        }

        // this is a very basic emulation of touch.
        // Tap, Hold, Moved, HorizontalDrag, FreeDrag, DragComplete
        private void EmulateGesturesWithMouse(bool isActive)
        {
            if (!isActive) return;
            if (TouchPanel.EnabledGestures == GestureType.None) return;
            if (_mouseTouchLocation.State == TouchLocationState.Invalid) return;
            //if (mouseTouchLocation.Position.X < 0 || mouseTouchLocation.Position.Y < 0 ||
            //    mouseTouchLocation.Position.X >= TouchPanel.DisplayWidth || 
            //    mouseTouchLocation.Position.Y >= TouchPanel.DisplayHeight) return;
            
            Vector2 delta = _mouseTouchLocation.Position-_prevMouseTouchLocation.Position;

            bool pressing = _mouseTouchLocation.State == TouchLocationState.Pressed && (_prevMouseTouchLocation.State == TouchLocationState.Released || _prevMouseTouchLocation.State == TouchLocationState.Invalid);
            bool pressed = _mouseTouchLocation.State == TouchLocationState.Moved && (_prevMouseTouchLocation.State == TouchLocationState.Pressed || _prevMouseTouchLocation.State == TouchLocationState.Moved);
            bool releasing = _mouseTouchLocation.State == TouchLocationState.Released && (_prevMouseTouchLocation.State == TouchLocationState.Pressed || _prevMouseTouchLocation.State == TouchLocationState.Moved);
            bool released = _mouseTouchLocation.State == TouchLocationState.Released && (_prevMouseTouchLocation.State == TouchLocationState.Released || _prevMouseTouchLocation.State == TouchLocationState.Invalid);

            if(pressing) _pressTimestamp = DateTime.Now;
            TimeSpan pressDuration = (DateTime.Now-_pressTimestamp);

            /*
            if (mouseTouchLocation.State == TouchLocationState.Pressed)
            {
                if (TouchPanel.EnabledGestures == GestureType.Tap)
                {   //if only tap was expected, return now
                    gestures.Add(new GestureSample(GestureType.Tap,TimeSpan.Zero,
                        mouseTouchLocation.Position,Vector2.Zero,
                        Vector2.Zero,Vector2.Zero));
                    IsGestureAvailable = true;
                }
            }
            */
            TimeSpan maxTapDuration = TimeSpan.FromMilliseconds(600);

            if (releasing)
            {
                if ((TouchPanel.EnabledGestures & GestureType.Tap) != GestureType.None && delta == Vector2.Zero && pressDuration < maxTapDuration)
                {
                    _gestures.Add(new GestureSample(GestureType.Tap, TimeSpan.Zero,
                        _mouseTouchLocation.Position, Vector2.Zero,
                        Vector2.Zero, Vector2.Zero));
                    IsGestureAvailable = true;
                }
            }

            if (_mouseTouchLocation.State == TouchLocationState.Moved && _prevMouseTouchLocation.State == TouchLocationState.Moved)
            {
                if ((TouchPanel.EnabledGestures & GestureType.Hold) != GestureType.None && delta == Vector2.Zero)
                {
                    _gestures.Add(new GestureSample(GestureType.Hold, TimeSpan.Zero,
                        _mouseTouchLocation.Position, Vector2.Zero,
                        Vector2.Zero, Vector2.Zero));
                    IsGestureAvailable = true;
                }
            }

            if (_mouseTouchLocation.State == TouchLocationState.Moved)
            {
                if ((TouchPanel.EnabledGestures & GestureType.HorizontalDrag) != GestureType.None && delta.X != 0)
                {
                    _gestures.Add(new GestureSample(GestureType.HorizontalDrag, TimeSpan.Zero,
                        _mouseTouchLocation.Position, Vector2.Zero,
                        delta, Vector2.Zero));
                    IsGestureAvailable = true;
                }
                if ((TouchPanel.EnabledGestures & GestureType.FreeDrag) != GestureType.None && delta != Vector2.Zero)
                {
                    _gestures.Add(new GestureSample(GestureType.FreeDrag, TimeSpan.Zero,
                        _mouseTouchLocation.Position, Vector2.Zero,
                        delta, Vector2.Zero));
                    IsGestureAvailable = true;
                }
            }

            if (_mouseTouchLocation.State == TouchLocationState.Released && _prevMouseTouchLocation.State == TouchLocationState.Moved)
            {                          
                if ((TouchPanel.EnabledGestures & GestureType.DragComplete) != GestureType.None)
                {
                    _gestures.Add(new GestureSample(GestureType.DragComplete, TimeSpan.Zero,
                        _mouseTouchLocation.Position, Vector2.Zero,
                        Vector2.Zero, Vector2.Zero));
                    IsGestureAvailable = true;
                }
                
            }


            return;
        }


        public bool IsButtonPressed(Buttons button)
        {
            return (_gamePadState.IsButtonDown(button) && _prevGamePadState.IsButtonUp(button));
        }
        public bool IsButtonReleased(Buttons button)
        {
            return (_gamePadState.IsButtonUp(button) && _prevGamePadState.IsButtonDown(button));
        }
        public bool IsButtonUp(Buttons button)
        {
            return (_gamePadState.IsButtonUp(button) && _prevGamePadState.IsButtonUp(button));
        }
        public bool IsButtonDown(Buttons button)
        {
            return (_gamePadState.IsButtonDown(button) && _prevGamePadState.IsButtonDown(button));
        }
        
        public bool IsKeyPressed(Keys key)
        {   
            return (_keyboardState.IsKeyDown(key) && _prevKeyboardState.IsKeyUp(key));
        }
        public bool IsKeyReleased(Keys key)
        {
            return (_keyboardState.IsKeyUp(key)   && _prevKeyboardState.IsKeyDown(key));
        }
        public bool IsKeyUp(Keys key)
        {
            return (_keyboardState.IsKeyUp(key)   && _prevKeyboardState.IsKeyUp(key));
        }
        public bool IsKeyDown(Keys key)
        {
            return (_keyboardState.IsKeyDown(key) && _prevKeyboardState.IsKeyDown(key));
        }

        
    }
}
