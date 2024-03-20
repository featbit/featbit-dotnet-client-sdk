﻿using System.Collections.Generic;

namespace FeatBit.ClientSdk
{
    public class FbIdentity
    {
        public readonly string Key;
        public readonly string Name;
        public readonly Dictionary<string, string> Custom;

        internal FbIdentity(string key, string name, Dictionary<string, string> custom)
        {
            Key = key;
            Name = name;
            Custom = custom;
        }

        /// <summary>
        /// Creates an <see cref="IFbUserBuilder"/> for constructing a user object using a fluent syntax.
        /// </summary>
        /// <remarks>
        /// This is the only method for building a <see cref="FbIdentity"/>. The <see cref="IFbUserBuilder"/> has methods
        /// for setting any number of properties, after which you call <see cref="IFbUserBuilder.Build"/> to get the
        /// resulting <see cref="FbIdentity"/> instance.
        /// </remarks>
        /// <example>
        /// <code>
        ///     var user = FbUser.Builder("a-unique-key-of-user")
        ///         .Name("user-name")
        ///         .Custom("email", "test@example.com")
        ///         .Build();
        /// </code>
        /// </example>
        /// <param name="key">a <see langword="string"/> that uniquely identifies a user</param>
        /// <returns>a builder object</returns>
        public static IFbUserBuilder Builder(string key)
        {
            return new FbUserBuilder(key);
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
