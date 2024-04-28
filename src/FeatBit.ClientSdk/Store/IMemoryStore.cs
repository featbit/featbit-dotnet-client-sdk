using System;
using System.Collections.Generic;
using FeatBit.Sdk.Client.Model;

namespace FeatBit.Sdk.Client.Store
{
    /// <summary>
    /// Interface for a data store that holds feature flags data received by the SDK.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementations must be thread-safe.
    /// </para>
    /// </remarks>
    public interface IMemoryStore
    {
        /// <summary>
        /// Retrieves a feature flag from the store, if available.
        /// </summary>
        /// <param name="id">the unique id of the flag within the store</param>
        /// <returns>The feature flag; null if the key is unknown.</returns>
        FeatureFlag Get(string id);

        /// <summary>
        /// Retrieves all the feature flags in the store.
        /// </summary>
        /// <returns>All feature flags.</returns>
        ICollection<FeatureFlag> GetAll();

        /// <summary>
        /// Attempts to update or insert a flag.
        /// </summary>
        /// <param name="flag">the feature flag</param>
        void Upsert(FeatureFlag flag);

        /// <summary>
        /// Triggers an event when a feature flag is changed.
        /// </summary>
        event EventHandler<FlagValueChangedEvent> FlagValueChanged;
    }
}