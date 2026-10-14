using RedisShop.Api.Common.Redis;
using RedisShop.Api.Features.Diagnostics;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedis(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks().AddRedis(
    builder.Configuration["Redis:ConnectionString"]!,
    name: "redis",
    tags: ["ready"]);

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapDiagnosticsEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
