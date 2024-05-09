using FeatBit.Sdk.Client.ChangeTracker;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Store;

namespace FeatBit.ClientSdk.Tests;

public class FlagTrackerTests
{
    void TestSubscriber(FlagValueChangedEvent theEvent)
    {
    }

    [Fact]
    public void DeduplicatedSubscriber()
    {
        var store = new DefaultMemoryStore(Array.Empty<FeatureFlag>());
        var tracker = new FlagTracker(store);

        for (var i = 0; i < 10; i++)
        {
            tracker.Subscribe(TestSubscriber);
        }

        Assert.Single(tracker.Subscribers);
    }

    [Fact]
    public void DeduplicateKeyedSubscriber()
    {
        var store = new DefaultMemoryStore(Array.Empty<FeatureFlag>());
        var tracker = new FlagTracker(store);

        for (var i = 0; i < 10; i++)
        {
            tracker.Subscribe("hello-world", TestSubscriber);
        }

        Assert.Single(tracker.KeyedSubscribers["hello-world"]);
    }

    [Fact]
    public void DeduplicateAnonymousSubscriber()
    {
        var store = new DefaultMemoryStore(Array.Empty<FeatureFlag>());
        var tracker = new FlagTracker(store);

        for (var i = 0; i < 10; i++)
        {
            tracker.Subscribe(changedEvent => { });
        }

        Assert.Single(tracker.Subscribers);
    }

    [Fact]
    public void DeduplicateAnonymousKeyedSubscriber()
    {
        var store = new DefaultMemoryStore(Array.Empty<FeatureFlag>());
        var tracker = new FlagTracker(store);

        for (var i = 0; i < 10; i++)
        {
            tracker.Subscribe("hello-world", changedEvent =>
            {
                Assert.Null(changedEvent.OldValue);
                Assert.Equal("true", changedEvent.NewValue);
                Assert.Equal("hello-world", changedEvent.Key);
            });
        }

        Assert.Single(tracker.KeyedSubscribers["hello-world"]);
    }
}