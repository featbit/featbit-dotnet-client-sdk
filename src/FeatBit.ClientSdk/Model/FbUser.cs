using System.Collections.Generic;

namespace FeatBit.ClientSdk.Model
{
    public sealed class FbUser
    {
        public readonly string Key;
        public readonly string Name;
        public readonly Dictionary<string, string> Custom;

        internal FbUser(string key, string name, Dictionary<string, string> custom)
        {
            Key = key;
            Name = name;
            Custom = custom;
        }

        /// <summary>
        /// Creates an <see cref="IFbUserBuilder"/> for constructing a user object using a fluent syntax.
        /// </summary>
        /// <remarks>
        /// This is the only method for building a <see cref="FbUser"/>. The <see cref="IFbUserBuilder"/> has methods
        /// for setting any number of properties, after which you call <see cref="IFbUserBuilder.Build"/> to get the
        /// resulting <see cref="FbUser"/> instance.
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

        internal object AsEndUser()
        {
            var customizedProperties = new List<object>();
            foreach (var custom in Custom)
            {
                customizedProperties.Add(new
                {
                    name = custom.Key,
                    value = custom.Value
                });
            }

            var endUser = new
            {
                keyId = Key,
                name = Name,
                customizedProperties
            };

            return endUser;
        }
    }
}