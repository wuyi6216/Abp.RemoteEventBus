using Castle.Core.Logging;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus.Kafka
{
    public class KafkaRemoteEventSubscriber : IRemoteEventSubscriber
    {
        public ILogger Logger { get; set; }

        private readonly ConcurrentDictionary<string, Consumer<Ignore, string>> _dictionary;

        private readonly IKafkaSetting _kafkaSetting;

        private bool _cancelled;

        private bool _disposed;

        public KafkaRemoteEventSubscriber(IKafkaSetting kafkaSetting)
        {
            Check.NotNullOrWhiteSpace(kafkaSetting.Properties["bootstrap.servers"] as string, "bootstrap.servers");

            _kafkaSetting = kafkaSetting;

            _dictionary = new ConcurrentDictionary<string, Consumer<Ignore, string>>();

            Logger = NullLogger.Instance;
        }

        public void Subscribe(IEnumerable<string> topics, Action<string, string> handler)
        {
            var existsTopics = topics.ToList().Where(p => _dictionary.ContainsKey(p));
            if (existsTopics.Any())
            {
                throw new AbpException(string.Format("Topics {0} have subscribed already", string.Join(",", existsTopics)));
            }

            topics.ToList().ForEach(topic =>
            {
                var consumer = new Consumer<Ignore, string>(_kafkaSetting.Properties, null, new StringDeserializer(Encoding.UTF8));

                _dictionary[topic] = consumer;

                consumer.Subscribe(topics);

                consumer.OnMessage += (_, msg) =>
                {
                    Logger.Debug($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

                    try
                    {
                        handler(msg.Topic, msg.Value);
                    }
                    catch(Exception ex)
                    {
                        Logger.Error($"Consume error", ex);
                    }

                    try
                    {
                        var committedOffsets = consumer.CommitAsync(msg).Result;

                        Logger.Debug($"Committed offset: {committedOffsets}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Commit error", ex);
                    }
                };

                consumer.OnError += (_, error) => Logger.Error($"Error: {error}");

                consumer.OnConsumeError += (_, error) => Logger.Error($"Consume error: {error}");

                Task.Factory.StartNew(() =>
                {
                    while (!_cancelled)
                    {
                        consumer.Poll(TimeSpan.FromMilliseconds(1000));
                    }
                });
            });
        }

        public Task SubscribeAsync(IEnumerable<string> topics, Action<string, string> handler)
        {
            return Task.Factory.StartNew(() =>
             {
                 Subscribe(topics, handler);
             });
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            _dictionary.Where(p => topics.Contains(p.Key)).Select(p => p.Value).ToList().ForEach(p => p.Unsubscribe());
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return Task.Factory.StartNew(() => Unsubscribe(topics));
        }

        public void UnsubscribeAll()
        {
            _dictionary.Select(p => p.Value).ToList().ForEach(p => p.Unsubscribe());
        }

        public Task UnsubscribeAllAsync()
        {
            return Task.Factory.StartNew(() => UnsubscribeAll());
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _cancelled = true;
                UnsubscribeAll();
                _dictionary.Select(p => p.Value).ToList().ForEach(consumer => consumer?.Dispose());

                _disposed = true;
            }
        }
    }
}
