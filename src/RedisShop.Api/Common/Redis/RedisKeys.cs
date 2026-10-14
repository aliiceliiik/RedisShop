namespace RedisShop.Api.Common.Redis
{
   
    public class RedisKeys
    {
        private const string Prefix = "redisshop";
        private const string Version = "v1";

        private static string Build(params string[] parts) => string.Join(':', [Prefix, Version, .. parts]);
        public static string Product(Guid id) => Build("product", id.ToString());
        public static string Ping() => Build("diag", "ping");
    }
}
