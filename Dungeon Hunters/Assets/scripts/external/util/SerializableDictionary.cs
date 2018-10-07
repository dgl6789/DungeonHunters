using UnityEngine;
using System;
using System.Collections.Generic;

namespace Numeralien.Utilities {
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] List<TKey> _k = new List<TKey>();
        [SerializeField] List<TValue> _v = new List<TValue>();

        //Unity doesn't know how to serialize a Dictionary
        [SerializeField] Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        public void OnBeforeSerialize() {
            _k.Clear();
            _v.Clear();

            foreach (KeyValuePair<TKey, TValue> p in _dict) {
                _k.Add(p.Key);
                _v.Add(p.Value);
            }
        }

        public void OnAfterDeserialize() {
            _dict = new Dictionary<TKey, TValue>();

            for (int i = 0; i != Mathf.Min(_k.Count, _v.Count); i++)
                _dict.Add(_k[i], _v[i]);
        }
    }
}