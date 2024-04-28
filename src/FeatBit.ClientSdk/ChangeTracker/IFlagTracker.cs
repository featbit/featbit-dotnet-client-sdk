using System;

namespace FeatBit.Sdk.Client.ChangeTracker
{
    /// <summary>
    /// An interface for tracking feature flag changes.
    /// </summary>
    public interface IFlagTracker : IDisposable
    {
        /// <summary>
        /// Adds a subscriber to the tracker.
        /// </summary>
        /// <param name="subscriber">The subscriber to add.</param>
        void Subscribe(Subscriber subscriber);

        /// <summary>
        /// Adds a subscriber to the tracker for a specific key.
        /// </summary>
        /// <param name="key">The flag key to subscribe to.</param>
        /// <param name="subscriber">The subscriber to add.</param>
        void Subscribe(string key, Subscriber subscriber);

        /// <summary>
        /// Removes a subscriber from the tracker.
        /// </summary>
        /// <param name="subscriber">The subscriber to remove.</param>
        void Unsubscribe(Subscriber subscriber);

        /// <summary>
        /// Removes a subscriber from the tracker for a specific key.
        /// </summary>
        /// <param name="key">The flag key to unsubscribe from.</param>
        /// <param name="subscriber">The subscriber to remove.</param>
        void Unsubscribe(string key, Subscriber subscriber);
    }
}