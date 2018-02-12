using Castle.Core.Logging;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus.Kafka
{
    public class KafkaRemoteEventPublisher : IRemoteEventPublisher
    {
        public ILogger Logger { get; set; }

        private readonly IKafkaSetting _kafkaSetting;
        private readonly IRemoteEventSerializer _remoteEventSerializer;

        private readonly Producer<Null, string> _producer;

        private bool _disposed;

        public KafkaRemoteEventPublisher(IKafkaSetting kafkaSetting, IRemoteEventSerializer remoteEventSerializer)
        {
            Check.NotNullOrWhiteSpace(kafkaSetting.Properties["bootstrap.servers"] as string, "bootstrap.servers");

            _kafkaSetting = kafkaSetting;
            _remoteEventSerializer = remoteEventSerializer;

            Logger = NullLogger.Instance;

            _producer = new Producer<Null, string>(_kafkaSetting.Properties, null, new StringSerializer(Encoding.UTF8));
        }

        public void Publish(string topic, IRemoteEventData remoteEventData)
        {
            PublishAsync(topic, remoteEventData);
            //_producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task PublishAsync(string topic, IRemoteEventData remoteEventData)
        {
            Logger.Debug($"{_producer.Name} producing on {topic}");

            var deliveryReport = _producer.ProduceAsync(topic, null, _remoteEventSerializer.Serialize(remoteEventData));
            return deliveryReport.ContinueWith(task =>
            {
                Logger.Debug($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _producer?.Dispose();
                _disposed = true;
            }
        }
    }
}