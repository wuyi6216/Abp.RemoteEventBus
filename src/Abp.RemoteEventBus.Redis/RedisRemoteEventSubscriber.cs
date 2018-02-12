using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Logging;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus.Redis
{
    public class RedisRemoteEventSubscriber : IRemoteEventSubscriber
    {
        private readonly List<string> _dictionary;

        private readonly ConnectionMultiplexer _connectionMultiplexer;

        private readonly IDatabase _database;

        private readonly ISubscriber _subscriber;

        public ILogger Logger;

        private bool _disposed;

        public RedisRemoteEventSubscriber(IRedisSetting redisSetting)
        {
            Check.NotNullOrWhiteSpace(redisSetting.Server, "redisSetting.Server");

            _connectionMultiplexer = ConnectionMultiplexer.Connect(redisSetting.Server);
            _database = _connectionMultiplexer.GetDatabase(redisSetting.DatabaseId);
            _subscriber = _database.Multiplexer.GetSubscriber();

            _dictionary = new List<string>();

            Logger = NullLogger.Instance;
        }

        public void Subscribe(IEnumerable<string> topics, Action<string, string> handler)
        {
            var existsTopics = topics.ToList().Where(topic => _dictionary.Contains(topic));
            if (existsTopics.Any())
            {
                throw new AbpException(string.Format("the topics {0} have subscribed already", string.Join(",", existsTopics)));
            }

            foreach (var topic in topics)
            {
                _subscriber.Subscribe(topic, (channel, message) => { handler(channel, message); });
                _dictionary.Add(topic);
            }
        }

        public async Task SubscribeAsync(IEnumerable<string> topics, Action<string, string> handler)
        {
            var tasks = new List<Task>();
            foreach (var topic in topics)
            {
                tasks.Add(_subscriber.SubscribeAsync(topic, (channel, message) => { handler(channel, message); }).ContinueWith(p=> _dictionary.Add(topic)));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            foreach (var topic in topics)
            {
                _subscriber.Unsubscribe(topic);
            }
        }

        public async Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            var tasks = new List<Task>();
            foreach (var topic in topics)
            {
                tasks.Add(_subscriber.UnsubscribeAsync(topic));
            }
            await Task.WhenAll(tasks.ToArray());
        }

        public Task UnsubscribeAllAsync()
        {
            return Task.Factory.StartNew(UnsubscribeAll);
        }

        public void UnsubscribeAll()
        {
            Unsubscribe(_dictionary);
        }

        public void Dispose()
        {
            if(!_disposed)
            {
                UnsubscribeAll();
                _connectionMultiplexer.Dispose();

                _disposed = true;
            }
        }
    }
}
