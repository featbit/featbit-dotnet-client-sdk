using System.Collections.Generic;
using FeatBit.Sdk.Client.Store;

namespace FeatBit.Sdk.Client.ChangeTracker
{
    public delegate void Subscriber(FlagValueChangedEvent theEvent);

    internal class FlagTracker : IFlagTracker
    {
        private readonly IMemoryStore _store;

        private readonly object _subscriberLock = new object();
        internal readonly List<Subscriber> Subscribers = new List<Subscriber>();

        private readonly object _keyedSubscriberLock = new object();
        internal readonly Dictionary<string, List<Subscriber>> KeyedSubscribers =
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
                if (!Subscribers.Contains(subscriber))
                {
                    Subscribers.Add(subscriber);
                }
            }
        }

        public void Subscribe(string key, Subscriber subscriber)
        {
            lock (_keyedSubscriberLock)
            {
                if (!KeyedSubscribers.TryGetValue(key, out var keyedSubscribers))
                {
                    keyedSubscribers = new List<Subscriber>();
                    KeyedSubscribers[key] = keyedSubscribers;
                }

                if (!keyedSubscribers.Contains(subscriber))
                {
                    keyedSubscribers.Add(subscriber);
                }
            }
        }

        public void Unsubscribe(Subscriber subscriber)
        {
            lock (_subscriberLock)
            {
                Subscribers.Remove(subscriber);
            }
        }

        public void Unsubscribe(string key, Subscriber subscriber)
        {
            lock (_keyedSubscriberLock)
            {
                if (KeyedSubscribers.TryGetValue(key, out var keyedSubscribers))
                {
                    keyedSubscribers.Remove(subscriber);
                }
            }
        }

        private void HandleFlagValueChangedEvent(object sender, FlagValueChangedEvent theEvent)
        {
            foreach (var subscriber in Subscribers)
            {
                subscriber.Invoke(theEvent);
            }

            if (KeyedSubscribers.TryGetValue(theEvent.Key, out var keyedSubscribers))
            {
                foreach (var subscriber in keyedSubscribers)
                {
                    subscriber.Invoke(theEvent);
                }
            }
        }

        public void Dispose()
        {
            Subscribers.Clear();
            KeyedSubscribers.Clear();
            _store.FlagValueChanged -= HandleFlagValueChangedEvent;
        }
    }
}