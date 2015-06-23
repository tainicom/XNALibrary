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

using System.Collections.Generic;

namespace tainicom.DataStructures.Collections
{
    public class Set<TValue> : ICollection<TValue>
    {
        private Dictionary<TValue, short> _dictionary;

        public Set()
        {
            _dictionary = new Dictionary<TValue,short>();
        }

        public void Add(TValue item)
        {
            _dictionary.Add(item,0);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(TValue item)
        {
            return _dictionary.ContainsKey(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _dictionary.Keys.CopyTo(array,arrayIndex);
        }

        public int Count
        {
            get { return _dictionary.Keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TValue item)
        {
            return _dictionary.Remove(item);
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }     

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }
    }
}
