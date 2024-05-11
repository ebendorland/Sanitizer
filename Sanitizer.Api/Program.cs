using Microsoft.OpenApi.Models;
using Sanitizer.Core.Interfaces;
using Sanitizer.Library.Repos;
using Sanitizer.Library.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => 
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    x.IncludeXmlComments(xmlPath);
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "Sanitizer", Version = "V1" });
});

builder.Services.AddSingleton<ISensitiveWordsRepo, MSSqlSanitizerRepo>();
builder.Services.AddSingleton<SanitizerService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
