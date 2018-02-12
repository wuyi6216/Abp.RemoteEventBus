using Abp.Json;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus.Redis
{
    public class RedisRemoteEventPublisher : IRemoteEventPublisher
    {
        private readonly IDatabase _database;

        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly IRemoteEventSerializer _remoteEventSerializer;
        
        private bool _disposed;

        public RedisRemoteEventPublisher(IRedisSetting redisSetting,IRemoteEventSerializer remoteEventSerializer)
        {
            Check.NotNullOrWhiteSpace(redisSetting.Server, "redisSetting.Server");

            _remoteEventSerializer = remoteEventSerializer;
            
            _connectionMultiplexer = ConnectionMultiplexer.Connect(redisSetting.Server);
            _database = _connectionMultiplexer.GetDatabase(redisSetting.DatabaseId);
        }

        public void Publish(string topic,IRemoteEventData remoteEventData)
        {
            _database.Publish(topic, _remoteEventSerializer.Serialize(remoteEventData));
        }

        public Task PublishAsync(string topic, IRemoteEventData remoteEventData)
        {
            return _database.PublishAsync(topic, remoteEventData.ToJsonString());
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _connectionMultiplexer.Dispose();

                _disposed = true;
            }
        }
    }
}
