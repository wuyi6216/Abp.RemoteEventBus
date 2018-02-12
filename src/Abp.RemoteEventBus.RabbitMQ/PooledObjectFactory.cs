using Abp.Dependency;
using Commons.Pool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus.RabbitMQ
{
    public class PooledObjectFactory : IPooledObjectFactory<IConnection>
    {
        private ConnectionFactory _connectionFactory;

        public PooledObjectFactory(IRabbitMQSetting rabbitMQSetting)
        {
            Check.NotNullOrWhiteSpace(rabbitMQSetting.Url, "Url");
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMQSetting.Url),
                AutomaticRecoveryEnabled = true
            };
        }

        public IConnection Create()
        {
            return _connectionFactory.CreateConnection();
        }

        public void Destroy(IConnection obj)
        {
            obj.Dispose();
        }
    }
}
