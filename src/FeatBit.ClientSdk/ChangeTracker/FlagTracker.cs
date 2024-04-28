using System;
using System.Collections.Generic;
using FeatBit.Sdk.Client.Store;

namespace FeatBit.Sdk.Client.ChangeTracker
{
    public delegate void Subscriber(FlagValueChangedEvent theEvent);

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

    internal class FlagTracker : IFlagTracker
    {
        private readonly IMemoryStore _store;

        private readonly object _subscriberLock = new object();
        private readonly List<Subscriber> _subscribers = new List<Subscriber>();

        private readonly object _keyedSubscriberLock = new object();
        private readonly Dictionary<string, List<Subscriber>> _keyedSubscribers =
            new Dictionary<string, List<Subscriber>>();

        public FlagTracker(IMemoryStore store)
        {
            _store = store;
            store.FlagValueChanged += HandleFlagValueChangedEvent;
        }

        public void Subscribe(Subscriber subscriber)
        {
            lock (_subscriberLock)
            {
                _subscribers.Add(subscriber);
            }
        }

        public void Subscribe(string key, Subscriber subscriber)
        {
            lock (_keyedSubscriberLock)
            {
                if (!_keyedSubscribers.TryGetValue(key, out var keyedSubscribers))
                {
                    keyedSubscribers = new List<Subscriber>();
                    _keyedSubscribers[key] = keyedSubscribers;
                }

                keyedSubscribers.Add(subscriber);
            }
        }

        public void Unsubscribe(Subscriber subscriber)
        {
            lock (_subscriberLock)
            {
                _subscribers.Remove(subscriber);
            }
        }

        public void Unsubscribe(string key, Subscriber subscriber)
        {
            lock (_keyedSubscriberLock)
            {
                if (_keyedSubscribers.TryGetValue(key, out var keyedSubscribers))
                {
                    keyedSubscribers.Remove(subscriber);
                }
            }
        }

        private void HandleFlagValueChangedEvent(object sender, FlagValueChangedEvent theEvent)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Invoke(theEvent);
            }

            if (_keyedSubscribers.TryGetValue(theEvent.Key, out var keyedSubscribers))
            {
                foreach (var subscriber in keyedSubscribers)
                {
                    subscriber.Invoke(theEvent);
                }
            }
        }

        public void Dispose()
        {
            _subscribers.Clear();
            _keyedSubscribers.Clear();
            _store.FlagValueChanged -= HandleFlagValueChangedEvent;
        }
    }
}