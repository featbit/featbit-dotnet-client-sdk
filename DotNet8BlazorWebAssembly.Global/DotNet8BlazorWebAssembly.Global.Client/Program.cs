using DotNet8BlazorWebAssembly.Global.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FeatBit.ClientSdk;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

//var consoleLoggerFactory = LoggerFactory.Create(x => x.AddConsole());
//var options = new FbOptionsBuilder("s0ZIrZbMfEuZGv3UrDtskAUVG0I5KKrEyYAqZtyI-5IQ")
//                            .Event(new Uri("https://featbit-tio-eu-eval.azurewebsites.net"))
//                            .Streaming(new Uri("wss://featbit-tio-eu-eval.azurewebsites.net"))
//                            .APIs(new Uri("https://featbit-tio-eu-api.azurewebsites.net"))
//                            .LoggerFactory(consoleLoggerFactory)
//                            .Build();
//var featureFlagStore = new FbClient(options);
//builder.Services.AddSingleton<IFbClient>(featureFlagStore);


await builder.Build().RunAsync();
