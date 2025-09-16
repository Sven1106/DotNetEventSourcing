using System.Text.Json.Serialization;
using EndpointsGenerator;
using KurrentDB.Client;
using KurrentExample;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// 2) Wire op i ASP.NET Core for HTTP
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = JsonDefaults.Options.PropertyNamingPolicy;
    o.SerializerOptions.PropertyNameCaseInsensitive = JsonDefaults.Options.PropertyNameCaseInsensitive;
    foreach (var c in JsonDefaults.Options.Converters)
        o.SerializerOptions.Converters.Add(c);
});

builder.Services.Configure<JsonOptions>(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonDefaults.Options.PropertyNamingPolicy;
    o.JsonSerializerOptions.PropertyNameCaseInsensitive = JsonDefaults.Options.PropertyNameCaseInsensitive;
    foreach (var c in JsonDefaults.Options.Converters)
        o.JsonSerializerOptions.Converters.Add(c);
});

const string connectionString =
    "kurrentdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";

var settings = KurrentDBClientSettings.Create(connectionString);
var client = new KurrentDBClient(settings);
builder.Services.AddSingleton(client);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "KurrentExample v1")
    );
}

app.UseHttpsRedirection();

app.MapAllEndpoints();

app.Run();