using EndpointsGenerator;
using KurrentDB.Client;
using KurrentExample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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