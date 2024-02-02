using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FeatBit.ClientSdk.Concurrent
{
    public class FeatureFlagsCollection<String, FeatureFlag>
    {
        private static FeatureFlagsCollection<String, FeatureFlag> _instance;
        private static readonly object _lock = new object();

        public ConcurrentDictionary<String, FeatureFlag> _dictionary;

        // Private constructor ensures that an object cannot be created outside of the class
        private FeatureFlagsCollection()
        {
            _dictionary = new ConcurrentDictionary<String, FeatureFlag>();
        }

        // Public static method to get the instance of the class
        public static FeatureFlagsCollection<String, FeatureFlag> Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FeatureFlagsCollection<String, FeatureFlag>();
                        }
                    }
                }
                return _instance;
            }
        }

        // Instance methods follow...

        public bool AddOrUpdate(String key, FeatureFlag value)
        {
            return _dictionary.AddOrUpdate(key, value, (existingKey, existingValue) => value) != null;
        }

        public bool TryGetValue(String key, out FeatureFlag value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public bool TryRemove(String key)
        {
            return _dictionary.TryRemove(key, out _);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public FeatureFlag this[String key]
        {
            get => _dictionary[key];
            set => AddOrUpdate(key, value);
        }

        public int Count => _dictionary.Count;
    }
}
