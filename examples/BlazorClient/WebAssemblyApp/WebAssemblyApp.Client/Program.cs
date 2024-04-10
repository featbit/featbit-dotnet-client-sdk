using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FeatBit.ClientSdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


var options = new FbOptionsBuilder("S37S_0bmkUKTQkCIg5GnKQ5ZjgdjXPU0qDo5LAVn4GzA")
                    .Eval(new Uri("https://featbit-tio-eval.zeabur.app"))
                    .LoggerFactory(NullLoggerFactory.Instance)
                    .DataSyncMethod(DataSyncMethodEnum.Polling, 5000)
                    .Build();
builder.Services.AddSingleton<IFbClient>(provider => new FbClient(options, autoSync: true));

await builder.Build().RunAsync();
