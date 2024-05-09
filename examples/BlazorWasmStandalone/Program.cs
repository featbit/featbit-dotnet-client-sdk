using BlazorWasmStandalone;
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// inject FbClient
await AddFeatBitAsync();

await builder.Build().RunAsync();

return;

async Task AddFeatBitAsync()
{
    var initialUser = FbUser.Builder("anonymous-id").Name("anonymous").Build();

    var options = new FbOptionsBuilder("JbmetT2IvU2CJTxObJLbiQ1XEjhWE6kEaf1IbJu7gTNQ")
        // set the pollingInterval to 5 seconds for testing purposes
        .Polling(new Uri("http://localhost:5100"), TimeSpan.FromSeconds(5))
        .Build();

    var fbClient = new FbClient(options, initialUser: initialUser);

    // start the client and wait for 3 seconds to initialize.
    await fbClient.StartAsync(TimeSpan.FromSeconds(3));

    // register the client as a singleton service
    builder.Services.AddSingleton<IFbClient>(fbClient);
}