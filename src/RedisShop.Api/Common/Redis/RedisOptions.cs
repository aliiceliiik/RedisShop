namespace RedisShop.Api.Common.Redis
{
    public class RedisOptions
    {
        public const string SectionName = "Redis";

        public required string ConnectionString { get; init; }
        public string KeyPrefix { get; init; } = "redisshop";
        public int DefaultDatabase { get; set; } = 0;

    }
}
