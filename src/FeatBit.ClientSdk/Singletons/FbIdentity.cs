using System;
using System.Collections.Generic;

namespace FeatBit.ClientSdk
{
    public class FbIdentity
    {
        private static readonly Lazy<FbIdentity> lazy =
        new Lazy<FbIdentity>(() => new FbIdentity());

        public static FbIdentity Instance { get { return lazy.Value; } }

        public string Key;
        public string Name;
        public Dictionary<string, string> Custom;

        private FbIdentity()
        {
        }

        public void Update(string key, string name, Dictionary<string, string> custom)
        {
            Key = key;
            Name = name;
            Custom = custom;
        }
    }
}
