using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace RedisShop.Api.Common.Redis
{
    public static class RedisServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddOptions<RedisOptions>().Bind(configuration.GetSection(RedisOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
                
                var logger = sp.GetRequiredService<ILogger<ConnectionMultiplexer>>();

                var config = ConfigurationOptions.Parse(options.ConnectionString);
                config.AllowAdmin = true;
                config.AbortOnConnectFail = false;
                config.ConnectRetry = 5;
                config.ConnectTimeout = 5_000;
                config.SyncTimeout = 5_000;
                config.KeepAlive = 60;
                config.DefaultDatabase = options.DefaultDatabase;
                config.ClientName = "redisshop-api";

                var multiplexer = ConnectionMultiplexer.Connect(config);

                multiplexer.ConnectionFailed += (_, e) =>
                {
                    logger.LogError(e.Exception, "Redis bağlantısı koptu: {Endpoint} / {FailureType}", e.EndPoint, e.FailureType);
                };
                multiplexer.ConnectionRestored += (_, e) =>
                {
                    logger.LogInformation("Redis bağlantısı geri geldi: {Endpoint}", e.EndPoint);
                };

                return multiplexer;
            });

            services.AddScoped(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            return services;
        }
    }
}
