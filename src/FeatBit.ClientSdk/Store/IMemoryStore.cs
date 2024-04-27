using System.Collections.Generic;
using FeatBit.ClientSdk.Model;

namespace FeatBit.ClientSdk.Store
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
        /// Overwrites the store's contents with a set of new items.
        /// </summary>
        /// <param name="flags">a list of <see cref="FeatureFlag"/> instances with
        /// their store keys.</param>
        void Populate(IEnumerable<FeatureFlag> flags);

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
    }
}