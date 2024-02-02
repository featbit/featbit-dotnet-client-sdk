using System;
using System.Collections.Generic;

namespace FeatBit.ClientSdk.Models
{
    public class FbIdentity
    {
        public readonly string Key;
        private string _name;
        public string Name { get {  return _name; } }
        public readonly Dictionary<string, string> Custom;

        public FbIdentity()
        {
            Key = Guid.NewGuid().ToString();
            _name = Key;
            Custom = new Dictionary<string, string>();
        }

        public FbIdentity(string key)
        {
            Key = key;
            _name = key;
            Custom = new Dictionary<string, string>();
        }

        public FbIdentity(string key, string name)
        {
            Key = key;
            _name = name;
            Custom = new Dictionary<string, string>();
        }

        public FbIdentity(string key, string name, Dictionary<string, string> custom)
        {
            Key = key;
            _name = name;
            Custom = custom;
        }

        public void AddProperty(string key, string value)
        {
            if (Custom.ContainsKey(key))
            {
                Custom[key] = value;
            }
            else
            {
                Custom.Add(key, value);
            }
        }

        public void UpdateProperty(string key, string value)
        {
            if (Custom.ContainsKey(key))
            {
                Custom[key] = value;
            }
            else
            {
                Custom.Add(key, value);
            }
        }

        public void UpdateName(string name)
        {
            _name = name;
        }

        public string ValueOf(string property)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                return string.Empty;
            }

            if (property == "keyId")
            {
                return Key;
            }

            if (property == "name")
            {
                return Name;
            }

            return Custom.TryGetValue(property, out var value) ? value : string.Empty;
        }
    }
}
