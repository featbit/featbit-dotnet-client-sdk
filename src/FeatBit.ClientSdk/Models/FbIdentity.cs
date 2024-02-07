using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FeatBit.ClientSdk.Models
{
    public class FbIdentity
    {
        private string _keyId;
        [JsonProperty("keyId")]
        public string Key { get { return _keyId; } }
        private string _name;
        [JsonProperty("name")]
        public string Name { get {  return _name; } }
        private readonly Dictionary<string, string> _custom;
        [JsonProperty("customizedProperties")]
        public Dictionary<string, string> Custom { get { return _custom; } }

        public FbIdentity()
        {
            _keyId = Guid.NewGuid().ToString();
            _name = Key;
            _custom = new Dictionary<string, string>();
        }

        public FbIdentity(string key)
        {
            _keyId = key;
            _name = key;
            _custom = new Dictionary<string, string>();
        }

        public FbIdentity(string key, string name)
        {
            _keyId = key;
            _name = name;
            _custom = new Dictionary<string, string>();
        }

        public FbIdentity(string key, string name, Dictionary<string, string> custom)
        {
            _keyId = key;
            _name = name;
            _custom = custom;
        }

        public void AddProperty(string key, string value)
        {
            if (_custom.ContainsKey(key))
            {
                _custom[key] = value;
            }
            else
            {
                _custom.Add(key, value);
            }
        }

        public void UpdateProperty(string key, string value)
        {
            if (_custom.ContainsKey(key))
            {
                _custom[key] = value;
            }
            else
            {
                _custom.Add(key, value);
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

            return _custom.TryGetValue(property, out var value) ? value : string.Empty;
        }
    }
}
