using RedisShop.Api.Common.Redis;
using StackExchange.Redis;
using System.Diagnostics;

namespace RedisShop.Api.Features.Diagnostics
{
    public static class DiagnosticsEndpoints
    {
        public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/diagnostics/redis", async (
                IConnectionMultiplexer multiplexer,
                IDatabase db,
                CancellationToken ct) =>
            {
                var sw = Stopwatch.StartNew();
                var latency = await db.PingAsync();
                sw.Stop();

                var key = RedisKeys.Ping();
                await db.StringSetAsync(key, DateTimeOffset.UtcNow.ToString("O"), TimeSpan.FromSeconds(30));
                var value = await db.StringGetAsync(key);
                var ttl = await db.KeyTimeToLiveAsync(key);

                var endpoint = multiplexer.GetEndPoints().First();
                var server = multiplexer.GetServer(endpoint);
                var info = await server.InfoAsync("server");
                var version = info.SelectMany(g => g).FirstOrDefault(p => p.Key == "redis_version").Value;

                return Results.Ok(new
                {
                    Connected = multiplexer.IsConnected,
                    RedisVersion = version,
                    PingLatencyMs = latency.TotalMilliseconds,
                    RoundTripMs = sw.Elapsed.TotalMilliseconds,
                    Key = key,
                    Value = value.ToString(),
                    TtlSeconds = ttl?.TotalSeconds
                });
            }).WithName("RedisDiagnostics");

            return app;
        }
    }
}
