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
using System.IO;
#if WP8_1 || W8_1 || W10
using Windows.Storage;
#elif ANDROID

#else
using System.IO.IsolatedStorage;
#endif

namespace tainicom.Storage
{
    public class StorageBinaryFile : IDisposable
    {
#if WP8_1 || W8_1 || W10
        StorageFolder storage;
        Stream stream;
#elif ANDROID
		string storage;
        Stream stream;
#else
        IsolatedStorageFile storage;
        IsolatedStorageFileStream stream;
#endif
        public BinaryWriter outStream;
        public BinaryReader inStream;
        public int version;

        #region IDisposable Members

        private StorageBinaryFile()
        {

        }

        ~StorageBinaryFile()
        {
            Dispose(false);
        }

#if WP8_1 || W8_1 || W10
        private static StorageFolder GetUserStore()
        {
            return ApplicationData.Current.LocalFolder;
        }
#elif WINDOWS
        private static IsolatedStorageFile GetUserStore()
        {
            return IsolatedStorageFile.GetUserStoreForDomain();
        }
#elif ANDROID
		private static string GetUserStore()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}
#else   
        private static IsolatedStorageFile GetUserStore()
        {         
            return IsolatedStorageFile.GetUserStoreForApplication();
        }
#endif


#if WP8_1 || W8_1 || W10
        static public StorageBinaryFile OpenFile(string filename)
        {
            StorageFolder storage = GetUserStore();
            if (storage == null) return null;
            StorageBinaryFile instance = null;

            try
            {
                instance = new StorageBinaryFile();
                instance.storage = storage;
                var openStreamTask = storage.OpenStreamForReadAsync(filename);
                openStreamTask.Wait();
                instance.stream = openStreamTask.Result;
                instance.inStream = new BinaryReader(instance.stream);
                instance.version = instance.inStream.ReadInt32();
            }
            catch (FileNotFoundException fnfe) { return null;}
            catch (AggregateException ae) {return null;}
            
            return instance;
        }
#elif ANDROID       
        static public StorageBinaryFile OpenFile(string filename)
        {
            var storage = GetUserStore();
            if (storage == null) return null;
            StorageBinaryFile instance = null;

            try
            {
                instance = new StorageBinaryFile();
                instance.storage = storage;
                string fullPath = Path.Combine(storage, filename);
                if (!File.Exists(fullPath)) return null;
                Stream stream = File.Open(fullPath, FileMode.Open, System.IO.FileAccess.ReadWrite);
                instance.stream = stream;
                instance.inStream = new BinaryReader(instance.stream);
                instance.version = instance.inStream.ReadInt32();
            }
            catch (FileNotFoundException fnfe) { return null;}
            catch (AggregateException ae) {return null;}

            return instance;
        }
#else
        static public StorageBinaryFile OpenFile(string filename)
        {   
            IsolatedStorageFile storage = GetUserStore();
            if (storage == null) return null;
            if (!storage.FileExists(filename) ) return null;
                
            StorageBinaryFile instance = null;

            instance = new StorageBinaryFile();
            instance.storage = storage;
            instance.stream = storage.OpenFile(filename, FileMode.Open);
            instance.inStream = new BinaryReader(instance.stream);
            instance.version = instance.inStream.ReadInt32();  // read format version
			
            return instance;
        }
#endif

#if WP8_1 || W8_1 || W10
        static public StorageBinaryFile CreateFile(string filename, int version)
        {
            StorageFolder storage = GetUserStore();
            if (storage == null) return null;
            StorageBinaryFile instance = null;

            instance = new StorageBinaryFile();
            instance.storage = storage;
            var openStreamTask = storage.OpenStreamForWriteAsync(filename,CreationCollisionOption.ReplaceExisting);
            openStreamTask.Wait();
            instance.stream = openStreamTask.Result;
            instance.outStream = new BinaryWriter(instance.stream);
            instance.outStream.Write((Int32)version);//format version
            instance.version = version;

            return instance;
        }
#elif ANDROID
        static public StorageBinaryFile CreateFile(string filename, int version)
        {
            var storage = GetUserStore();
            if (storage == null) return null;
            StorageBinaryFile instance = null;

            instance = new StorageBinaryFile();
            instance.storage = storage;
            string fullPath = Path.Combine(storage, filename);
            Stream stream = File.Open(fullPath, FileMode.Create, System.IO.FileAccess.ReadWrite);
            instance.stream = stream;
            instance.outStream = new BinaryWriter(instance.stream);
            instance.outStream.Write((Int32)version);//format version
            instance.version = version;

            return instance;
        }
#else
        static public StorageBinaryFile CreateFile(string filename, int version)
        {
            IsolatedStorageFile storage = GetUserStore();
            if (storage == null) return null;
            StorageBinaryFile instance = null;

            instance = new StorageBinaryFile();
            instance.storage = storage;
            instance.stream = storage.CreateFile(filename);
            instance.outStream = new BinaryWriter(instance.stream);
            instance.outStream.Write((Int32)version);//format version
            instance.version = version;

            return instance;
        }
#endif
        
        
#if WP8_1 || W8_1 || W10

        public static void DeleteFile(string filename)
        {
            StorageFolder storage = GetUserStore();
            if (storage == null) return;

            var fileTask = storage.GetFileAsync(filename);
            fileTask.AsTask().Wait();
            var deleteTask = fileTask.AsTask().Result.DeleteAsync();
            deleteTask.AsTask().Wait();
        }
#elif ANDROID
        public static void DeleteFile(string filename)
        {
            var storage = GetUserStore();
            if (storage == null) return;

            string fullPath = Path.Combine(storage, filename);
            File.Delete(fullPath);
        }
#else
        public static void DeleteFile(string filename)
        {
            IsolatedStorageFile storage = GetUserStore(); 
            if (storage == null) return;            
            if (!storage.FileExists(filename) ) return;

            storage.DeleteFile(filename);
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
            
            #if WP8_1 || W8_1 || W10
            #elif WINDOWS
            if (storage != null) storage.Close();
            #elif ANDROID

            #else
            if (outStream != null) outStream.Close();
            if (inStream != null) inStream.Close();
            if (stream != null) stream.Close();
            #endif
            
            if (disposing)
            {
                if (outStream != null) outStream.Dispose();
                if (inStream != null) inStream.Dispose();
                if (stream != null) stream.Dispose();
                #if WP8_1 || W8_1 || W10                
                #elif ANDROID
        
                #else 
                if (storage != null) storage.Dispose();
                #endif
            }

            disposed = true;
        }

        #endregion
    }
}
