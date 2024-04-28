namespace FeatBit.Sdk.Client.Store
{
    /// <summary>
    /// An event that is raised when the value of a feature flag changes.
    /// </summary>
    public sealed class FlagValueChangedEvent
    {
        /// <summary>
        /// The key of the feature flag whose value has changed.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The last known value of the flag for the specified user prior to the update.
        /// </summary>
        public string OldValue { get; }

        /// <summary>
        /// The new value of the flag for the specified user.
        /// </summary>
        public string NewValue { get; }

        public FlagValueChangedEvent(string key, string oldValue, string newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}