using BlazorDownloadFile;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using WordToHtml;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorDownloadFile();
builder.Services.AddScoped<DialogService>();

builder.Logging.SetMinimumLevel(LogLevel.Debug);

await builder.Build().RunAsync();
