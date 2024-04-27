using WebAssemblyApp.Components;
using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var options = new FbOptionsBuilder("").Build();
var user = FbUser.Builder("tester").Build();
var fbClient = new FbClient(options, user);

builder.Services.AddSingleton<IFbClient>(fbClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebAssemblyApp.Client._Imports).Assembly);

app.Run();