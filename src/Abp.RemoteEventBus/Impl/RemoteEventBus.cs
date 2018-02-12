using Abp.RemoteEventBus.Exceptions;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Events.Bus;
using Abp.RemoteEventBus.Events;

namespace Abp.RemoteEventBus
{
    public class RemoteEventBus : IRemoteEventBus
    {
        public ILogger Logger { get; set; }

        private readonly IEventBus _eventBus;
        private readonly IRemoteEventPublisher _publisher;
        private readonly IRemoteEventSubscriber _subscriber;
        private readonly IRemoteEventTopicSelector _topicSelector;
        private readonly IRemoteEventSerializer _remoteEventSerializer;

        private bool _disposed;

        public RemoteEventBus(
            IEventBus eventBus,
            IRemoteEventPublisher publisher,
            IRemoteEventSubscriber subscriber,
            IRemoteEventTopicSelector topicSelector,
            IRemoteEventSerializer remoteEventSerializer
        )
        {
            _eventBus = eventBus;
            _publisher = publisher;
            _subscriber = subscriber;
            _topicSelector = topicSelector;
            _remoteEventSerializer = remoteEventSerializer;

            Logger = NullLogger.Instance;
        }

        public void Publish(IRemoteEventData eventData)
        {
            Publish(_topicSelector.SelectTopic(eventData), eventData);
        }

        public Task PublishAsync(IRemoteEventData eventData)
        {
            return PublishAsync(_topicSelector.SelectTopic(eventData), eventData);
        }

        public void Publish(string topic, IRemoteEventData eventData)
        {
            _eventBus.Trigger(this, new RemoteEventBusPublishingEvent(eventData));
            _publisher.Publish(topic, eventData);
            Logger.Debug($"Event published on topic {topic}");
            _eventBus.Trigger(this, new RemoteEventBusPublishedEvent(eventData));
        }

        public async Task PublishAsync(string topic, IRemoteEventData eventData)
        {
            await _eventBus.TriggerAsync(this, new RemoteEventBusPublishingEvent(eventData));
            await _publisher.PublishAsync(topic, eventData)
                .ContinueWith((s) => Logger.Debug($"Event published on topic {topic}"));
            await _eventBus.TriggerAsync(this, new RemoteEventBusPublishedEvent(eventData));
            await Task.FromResult(0);
        }

        public void Subscribe(string topic)
        {
            Subscribe(new[] {topic});
        }

        public Task SubscribeAsync(string topic)
        {
            return SubscribeAsync(new[] {topic});
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            _subscriber.Subscribe(topics, MessageHandle);
            Logger.Debug($"Subscribed topics {string.Join(",", topics)}");
        }

        public Task SubscribeAsync(IEnumerable<string> topics)
        {
            return _subscriber.SubscribeAsync(topics, MessageHandle)
                .ContinueWith((s) => Logger.Debug($"Subscribed topics {string.Join(",", topics)}"));
        }

        public void Unsubscribe(string topic)
        {
            Unsubscribe(new[] {topic});
        }

        public Task UnsubscribeAsync(string topic)
        {
            return UnsubscribeAsync(new[] {topic});
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            _subscriber.Unsubscribe(topics);
            Logger.Debug($"Unsubscribed topics {string.Join(",", topics)}");
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return _subscriber.UnsubscribeAsync(topics)
                .ContinueWith((s) => Logger.Debug($"Unsubscribed topics {string.Join(",", topics)}"));
        }

        public virtual void MessageHandle(string topic, string message)
        {
            Logger.Debug($"Receive message on topic {topic}");
            try
            {
                var eventData = _remoteEventSerializer.Deserialize<RemoteEventData>(message);
                var eventArgs = new RemoteEventArgs(eventData, topic, message);
                _eventBus.Trigger(this, new RemoteEventBusHandlingEvent(eventArgs));
                _eventBus.Trigger(this, eventArgs);
                _eventBus.Trigger(this, new RemoteEventBusHandledEvent(eventArgs));
            }
            catch (Exception ex)
            {
                Logger.Error("Consume remote message exception", ex);
                _eventBus.Trigger(this, new RemoteEventMessageHandleExceptionData(ex, topic, topic));
            }
        }

        public void UnsubscribeAll()
        {
            _subscriber.UnsubscribeAll();
            Logger.Debug($"Unsubscribed all topics");
        }

        public Task UnsubscribeAllAsync()
        {
            return _subscriber.UnsubscribeAllAsync()
                .ContinueWith((s) => Logger.Debug($"Unsubscribes all topics"));
            ;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                _subscriber?.Dispose();
                _publisher?.Dispose();

                _disposed = true;
            }
        }
    }
}