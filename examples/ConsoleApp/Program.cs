using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;

// setup SDK options
var options = new FbOptionsBuilder("<replace-with-your-env-secret>")
    .Polling(new Uri("<replace-with-your-polling-url>"), TimeSpan.FromMinutes(5))
    .Event(new Uri("<replace-with-your-event-url>"))
    .Build();

// use the anonymous user as the initial user
var anonymousUser = FbUser.Builder("anonymous")
    .Name("anonymous")
    .Custom("role", "visitor")
    .Build();

// Creates a new client instance that connects to FeatBit with the custom option.
var client = new FbClient(options, anonymousUser);
if (!client.Initialized)
{
    Console.WriteLine("FbClient failed to initialize. All Variation calls will use fallback value.");
}
else
{
    Console.WriteLine("FbClient successfully initialized!");
}

// after user logged in, call IdentifyAsync to switch the user and get the latest feature flags for the user
var authenticatedUser = FbUser.Builder("a-unique-key-of-bob")
    .Name("bob")
    .Custom("country", "FR")
    .Build();
await client.IdentifyAsync(authenticatedUser);

// flag to be evaluated
const string flagKey = "game-runner";

// evaluate a boolean flag for the user
var boolVariation = client.BoolVariation(flagKey, defaultValue: false);
Console.WriteLine($"flag '{flagKey}' returns {boolVariation} for user {authenticatedUser.Key}");

// evaluate a boolean flag for the user with reason
var boolVariationDetail = client.BoolVariationDetail(flagKey, defaultValue: false);
Console.WriteLine(
    $"flag '{flagKey}' returns {boolVariationDetail.Value} for user {authenticatedUser.Key}. " +
    $"Reason Description: {boolVariationDetail.Reason}"
);

// subscribe to flag changes
var flagTracker = client.FlagTracker;
flagTracker.Subscribe(flagKey, @event =>
{
    Console.WriteLine(
        "Flag value for '{0}' has changed from '{1}' to '{2}'",
        @event.Key,
        @event.OldValue,
        @event.NewValue
    );
});