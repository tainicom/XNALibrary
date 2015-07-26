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
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
#if IOS
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;
#if ES11
using OpenTK.Graphics.ES11;
#else
using OpenTK.Graphics.ES20;
#endif
#elif WINDOWS || LINUX || ANGLE
using OpenTK.Graphics;
using OpenTK.Platform;
using OpenTK;
using OpenTK.Graphics.OpenGL;
#endif
#if WP8
using System.Windows;
#endif

namespace tainicom.Helpers
{

    public class Threading
    {
        public const int kMaxWaitForUIThread = 750; // In milliseconds

#if !WP8
        static int mainThreadId;
#endif

#if ANDROID
        static List<Action> actions = new List<Action>();
        //static Mutex actionsMutex = new Mutex();
#elif IOS
        public static EAGLContext BackgroundContext;
#elif WINDOWS || LINUX || ANGLE
        public static IGraphicsContext BackgroundContext;
        public static IWindowInfo WindowInfo;
#endif

#if !WP8
        static Threading()
        {
#if NETFX_CORE
            mainThreadId = Environment.CurrentManagedThreadId;
#else
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
#endif
        }
#endif

        /// <summary>
        /// Checks if the code is currently running on the UI thread.
        /// </summary>
        /// <returns>true if the code is currently running on the UI thread.</returns>
        public static bool IsOnUIThread()
        {
#if WP8
            return Deployment.Current.Dispatcher.CheckAccess();
#elif NETFX_CORE
            return (mainThreadId == Environment.CurrentManagedThreadId);
#else
            return mainThreadId == Thread.CurrentThread.ManagedThreadId;
#endif
        }

        /// <summary>
        /// Throws an exception if the code is not currently running on the UI thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the code is not currently running on the UI thread.</exception>
        public static void EnsureUIThread()
        {
#if WP8
            if (!Deployment.Current.Dispatcher.CheckAccess())
#elif NETFX_CORE
            if (mainThreadId != Environment.CurrentManagedThreadId)
#else
            if (mainThreadId != Thread.CurrentThread.ManagedThreadId)
#endif
                throw new InvalidOperationException("Operation not called on UI thread.");
        }

#if WP8
        internal static void RunOnUIThread(Action action)
        {
            RunOnContainerThread(Deployment.Current.Dispatcher, action);
        }
        
        internal static void RunOnContainerThread(System.Windows.Threading.Dispatcher target, Action action)
        {
            target.BeginInvoke(action);
        }

        internal static void BlockOnContainerThread(System.Windows.Threading.Dispatcher target, Action action)
        {
            if (target.CheckAccess())
            {
                action();
            }
            else
            {
                EventWaitHandle wait = new AutoResetEvent(false);
                target.BeginInvoke(() =>
                {
                    action();
                    wait.Set();
                });
                wait.WaitOne(kMaxWaitForUIThread);
            }
        }
#endif

        /// <summary>
        /// Runs the given action on the UI thread and blocks the current thread while the action is running.
        /// If the current thread is the UI thread, the action will run immediately.
        /// </summary>
        /// <param name="action">The action to be run on the UI thread</param>
        public static void BlockOnUIThread(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

#if (NETFX_CORE) || PSM
            action();
#else
            // If we are already on the UI thread, just call the action and be done with it
            if (IsOnUIThread())
            {
                try
                {
                    action();
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Need to be on a different thread
#if WP8
                    BlockOnContainerThread(Deployment.Current.Dispatcher, action);
#else
                    throw (ex);
#endif
                }
                return;
            }

#if IOS
            lock (BackgroundContext)
            {
                // Make the context current on this thread if it is not already
                if (!Object.ReferenceEquals(EAGLContext.CurrentContext, BackgroundContext))
                    EAGLContext.SetCurrentContext(BackgroundContext);
                // Execute the action
                action();
                // Must flush the GL calls so the GPU asset is ready for the main context to use it
                GL.Flush();
                GraphicsExtensions.CheckGLError();
            }
#elif NETFX_CORE || LINUX || ANGLE
            lock (BackgroundContext)
            {
                // Make the context current on this thread
                BackgroundContext.MakeCurrent(WindowInfo);
                // Execute the action
                action();
                // Must flush the GL calls so the texture is ready for the main context to use
                GL.Flush();
                GraphicsExtensions.CheckGLError();
                // Must make the context not current on this thread or the next thread will get error 170 from the MakeCurrent call
                BackgroundContext.MakeCurrent(null);
            }
#elif WP8
            BlockOnContainerThread(Deployment.Current.Dispatcher, action);
#else
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
#if MONOMAC
            MonoMac.AppKit.NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
#else
            Add(() =>
#endif
            {
#if ANDROID
                //if (!Game.Instance.Window.GraphicsContext.IsCurrent)
                ((AndroidGameWindow)Game.Instance.Window).GameView.MakeCurrent();
#endif
                action();
                resetEvent.Set();
            });
            resetEvent.Wait();
#endif
#endif
        }

#if ANDROID
        static void Add(Action action)
        {
            lock (actions)
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// Runs all pending actions.  Must be called from the UI thread.
        /// </summary>
        internal static void Run()
        {
            EnsureUIThread();

            lock (actions)
            {
                foreach (Action action in actions)
                {
                    action();
                }
                actions.Clear();
            }
        }
#endif
    }
}
