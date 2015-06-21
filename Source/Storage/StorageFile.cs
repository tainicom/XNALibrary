﻿#region License 
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
using System.IO;
#if NETFX_CORE
using Windows.Storage;
#else
using System.IO.IsolatedStorage;
#endif

namespace tainicom.Storage
{
    public class StorageFile : IDisposable
    {
#if NETFX_CORE
        StorageFolder storage;
        Stream stream;
#else
        IsolatedStorageFile storage;
        IsolatedStorageFileStream stream;
#endif

        public Stream Stream { get { return stream; } }

        #region IDisposable Members

        private StorageFile()
        {

        }

        ~StorageFile()
        {
            Dispose(false);
        }

#if NETFX_CORE
        private static StorageFolder GetUserStore()
        {
            return ApplicationData.Current.LocalFolder;
        }
#elif WINDOWS
        private static IsolatedStorageFile GetUserStore()
        {
            return IsolatedStorageFile.GetUserStoreForDomain();
        }
#else   
        private static IsolatedStorageFile GetUserStore()
        {         
            return IsolatedStorageFile.GetUserStoreForApplication();
        }
#endif

        #if (!NETFX_CORE)
        static public StorageFile OpenFile(string filename)
        {   
            IsolatedStorageFile storage = GetUserStore();
            if (storage == null) return null;
            if (!storage.FileExists(filename) ) return null;
                
            StorageFile instance = null;

            instance = new StorageFile();
            instance.storage = storage;
            instance.stream = storage.OpenFile(filename, FileMode.Open);
            
            return instance;
        }
        #else
        static public StorageFile OpenFile(string filename)
        {
            StorageFolder storage = GetUserStore();
            if (storage == null) return null;
            StorageFile instance = null;

            try
            {
                instance = new StorageFile();
                instance.storage = storage;
                var openStreamTask = storage.OpenStreamForReadAsync(filename);
                openStreamTask.Wait();
                instance.stream = openStreamTask.Result;                
            }
            catch (FileNotFoundException fnfe) { return null;}
            catch (AggregateException ae) {return null;}
            
            return instance;
        }
        #endif
        
        #if (!NETFX_CORE)
        static public StorageFile CreateFile(string filename)
        {
            IsolatedStorageFile storage = GetUserStore();
            if (storage == null) return null;
            StorageFile instance = null;

            instance = new StorageFile();
            instance.storage = storage;
            instance.stream = storage.CreateFile(filename);

            return instance;
        }
        #else
        static public StorageFile CreateFile(string filename)
        {
            StorageFolder storage = GetUserStore();
            if (storage == null) return null;
            StorageFile instance = null;

            instance = new StorageFile();
            instance.storage = storage;
            var openStreamTask = storage.OpenStreamForWriteAsync(filename,CreationCollisionOption.ReplaceExisting);
            openStreamTask.Wait();
            instance.stream = openStreamTask.Result;
            
            return instance;
        }
        #endif
        
        #if (!NETFX_CORE)
        public static void DeleteFile(string filename)
        {
            IsolatedStorageFile storage = GetUserStore(); 
            if (storage == null) return;            
            if (!storage.FileExists(filename) ) return;

            storage.DeleteFile(filename);
        }
        #else
        public static void DeleteFile(string filename)
        {
            StorageFolder storage = GetUserStore();
            if (storage == null) return;

            var fileTask = storage.GetFileAsync(filename);
            fileTask.AsTask().Wait();
            var deleteTask = fileTask.AsTask().Result.DeleteAsync();
            deleteTask.AsTask().Wait();
        }
        #endif

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        private bool disposed = false;
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            
            #if (!NETFX_CORE)
            if (stream != null) stream.Close();
            #endif
            #if (WINDOWS)
            if (storage != null) storage.Close();
            #endif

            if (disposing)
            {               
                if (stream != null) stream.Dispose();
                #if (!NETFX_CORE)
                if (storage != null) storage.Dispose();
                #endif
            }

            disposed = true;
        }

        #endregion

    }
}
