using Microsoft.Identity.Client;

namespace backend.Model
{
    public class RedisConfiguration
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; }
    }
}
