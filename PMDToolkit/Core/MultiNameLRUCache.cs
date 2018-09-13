﻿/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace PMDToolkit.Core
{
    public class MultiNameLRUCache<K, V>
    {
        #region Fields

        private Dictionary<K, ValueItem<K, V>> names = new Dictionary<K, ValueItem<K, V>>();
        private LinkedList<ValueItem<K, V>> values = new LinkedList<ValueItem<K, V>>();
        private int capacity;
        private Object lockObject = new object();

        #endregion Fields

        public delegate void ItemRemovedEvent(V value);

        public ItemRemovedEvent OnItemRemoved;

        #region Constructors

        public MultiNameLRUCache(int capacity)
        {
            this.capacity = capacity;
        }

        #endregion Constructors

        #region Methods

        public void Add(K key, V val)
        {
            lock (lockObject)
            {
                //remove excesses
                if (values.Count >= capacity)
                {
                    RemoveFirst();
                }
                //add to cache as normal
                ValueItem<K, V> cacheItem = new ValueItem<K, V>(val);
                cacheItem.keys.AddLast(key);
                values.AddLast(cacheItem);
                names.Add(key, cacheItem);
            }
        }

        public void AddAlias(K newKey, K oldKey)
        {
            lock (lockObject)
            {
                ValueItem<K, V> node;
                //make sure key exists
                if (names.TryGetValue(oldKey, out node))
                {
                    node.keys.AddLast(newKey);
                    names.Add(newKey, node);
                }
            }
        }

        public K GetOriginalKeyFromAlias(K aliasKey)
        {
            lock (lockObject)
            {
                ValueItem<K, V> node;
                //make sure key exists
                if (names.TryGetValue(aliasKey, out node))
                {
                    return node.keys.First.Value;
                }
                return default(K);
            }
        }

        public bool ContainsKey(K key)
        {
            lock (lockObject)
            {
                return names.ContainsKey(key);
            }
        }

        public V Get(K key)
        {
            lock (lockObject)
            {
                ValueItem<K, V> node;
                if (names.TryGetValue(key, out node))
                {
                    V value = node.value;

                    values.Remove(node);
                    values.AddLast(node);
                    return value;
                }
                return default(V);
            }
        }

        protected void RemoveFirst()
        {
            // Remove from values
            LinkedListNode<ValueItem<K, V>> node = values.First;
            values.RemoveFirst();
            // Remove from keys
            foreach (K key in node.Value.keys)
            {
                names.Remove(key);
            }
            OnItemRemoved(node.Value.value);
        }

        public void Clear()
        {
            while (values.Count > 0)
            {
                RemoveFirst();
            }
        }

        #endregion Methods
    }

    internal class NameItem<K, V>
    {
        #region Fields

        public K key;
        public ValueItem<K, V> value;

        #endregion Fields

        #region Constructors

        public NameItem(K k, ValueItem<K, V> v)
        {
            key = k;
            value = v;
        }

        #endregion Constructors
    }

    internal class ValueItem<K, V>
    {
        #region Fields

        public V value;
        public LinkedList<K> keys;

        #endregion Fields

        #region Constructors

        public ValueItem(V v)
        {
            value = v;
            keys = new LinkedList<K>();
        }

        #endregion Constructors
    }
}