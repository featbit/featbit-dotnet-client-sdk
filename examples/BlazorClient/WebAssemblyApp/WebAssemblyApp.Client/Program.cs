using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var options = new FbOptionsBuilder("S37S_0bmkUKTQkCIg5GnKQ5ZjgdjXPU0qDo5LAVn4GzA")
    .Polling(new Uri("https://featbit-tio-eval.zeabur.app"))
    .Build();
var user = FbUser.Builder("tester").Build();
var fbClient = new FbClient(options, user);
builder.Services.AddSingleton<IFbClient>(fbClient);

await builder.Build().RunAsync();