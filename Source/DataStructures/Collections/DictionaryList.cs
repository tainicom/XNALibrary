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
using System.Collections;
using System.Collections.Generic;

namespace tainicom.DataStructures.Collections
{
    public class DictionaryList<TKey, TValue>: IEnumerable
    {
        protected Dictionary<TKey, List<TValue>> lists;
        int capacityKeys;
        int capacityValues;

        public DictionaryList(int capacityKeys,int capacityValues)
        {
            this.capacityKeys = 8;
            this.capacityValues = 8;
            lists = new Dictionary<TKey, List<TValue>>(capacityKeys);
        }

        private List<TValue> GetList(TKey key)
        {
            if (!lists.ContainsKey(key))
                lists.Add(key, new List<TValue>(capacityValues));
            return lists[key];
        }

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            List<TValue> list = GetList(key);
            list.Add(value);
        }

        public bool ContainsKey(TKey key)
        {
            return lists.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return lists.Keys; }
        }

        public bool Remove(TKey key)
        {
            return lists.Remove(key);
        }

        public bool TryGetValue(TKey key, out List<TValue> value)
        {            
            if (!lists.ContainsKey(key)) value = lists[key];
            else value = null;
            return (value != null);
        }

        public List<TValue> this[TKey key]
        {
            get { return lists[key]; }
            set { throw new NotImplementedException(); }
        }

        #endregion

        public void Clear()
        {
            lists.Clear();
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lists.GetEnumerator();
        }

        #endregion
    }
}
