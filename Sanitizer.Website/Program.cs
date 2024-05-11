using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Sanitizer.Core.Interfaces;
using Sanitizer.Library.Repos;
using Sanitizer.Library.Services;
using Sanitizer.Website.Components;

var builder = WebApplication.CreateBuilder(args);
var apiAssembly = Assembly.Load("Sanitizer.Api");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers()
    .AddApplicationPart(apiAssembly);

builder.Services.AddSwaggerGen(x => 
{
    var assembly = Assembly.Load("Sanitizer.Api");

    var xmlFile = $"{apiAssembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    x.IncludeXmlComments(xmlPath);
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Sanitizer", Version = "V1" });
});
builder.Services.AddSingleton<ISensitiveWordsRepo, MSSqlSanitizerRepo>();
builder.Services.AddSingleton<SanitizerService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("SensitiveWordsApi", (serviceProvider, client) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var baseUri = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
    client.Timeout = TimeSpan.FromSeconds(5);
    client.BaseAddress = new Uri($"{baseUri}/api/SensitiveWords/");
}).ConfigurePrimaryHttpMessageHandler(messageHandler =>
{
    var handler = new HttpClientHandler();

    if (handler.SupportsAutomaticDecompression)
    {
        handler.AutomaticDecompression = DecompressionMethods.Deflate |
                                         DecompressionMethods.Brotli |
                                         DecompressionMethods.GZip;
    }
    return handler;
});

var app = builder.Build();
app.UseSwagger(x => x.RouteTemplate = "api/swagger/{documentname}/swagger.json");
app.UseSwaggerUI(x =>
{
    x.RoutePrefix = "api/swagger";
    x.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Sanitizer Api v1");
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    
    app.UseHsts();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});
app.Run();
