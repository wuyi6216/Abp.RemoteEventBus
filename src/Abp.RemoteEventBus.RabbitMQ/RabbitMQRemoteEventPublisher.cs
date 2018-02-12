using Commons.Pool;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus.RabbitMQ
{
    public class RabbitMQRemoteEventPublisher : IRemoteEventPublisher
    {
        private const string _exchangeTopic = "RemoteEventBus.Exchange.Topic";

        private readonly IObjectPool<IConnection> _connectionPool;
        
        private readonly IRemoteEventSerializer _remoteEventSerializer;

        private bool _disposed;

        public RabbitMQRemoteEventPublisher(
            IPoolManager poolManager, 
            IRabbitMQSetting rabbitMQSetting,
            IRemoteEventSerializer remoteEventSerializer
            )
        {
            _remoteEventSerializer = remoteEventSerializer;
            
            _connectionPool = poolManager.NewPool<IConnection>()
                                    .InitialSize(rabbitMQSetting.InitialSize)
                                    .MaxSize(rabbitMQSetting.MaxSize)
                                    .WithFactory(new PooledObjectFactory(rabbitMQSetting))
                                    .Instance();
        }

        public void Publish(string topic, IRemoteEventData remoteEventData)
        {
            var connection = _connectionPool.Acquire();
            try
            {
                var channel = connection.CreateModel();
                channel.ExchangeDeclare(_exchangeTopic, "topic",true);
                var body = Encoding.UTF8.GetBytes(_remoteEventSerializer.Serialize(remoteEventData));
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.BasicPublish(_exchangeTopic, topic, properties, body);
            }
            finally
            {
                _connectionPool.Return(connection);
            }
        }

        public Task PublishAsync(string topic, IRemoteEventData remoteEventData)
        {
            return Task.Factory.StartNew(() =>
            {
                Publish(topic, remoteEventData);
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _connectionPool.Dispose();

                _disposed = true;
            }
        }
    }
}
