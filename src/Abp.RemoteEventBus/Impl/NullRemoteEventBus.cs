using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus
{
    public class NullRemoteEventBus : IRemoteEventBus
    {
        public static NullRemoteEventBus Instance { get { return SingletonInstance; } }
        private static readonly NullRemoteEventBus SingletonInstance = new NullRemoteEventBus();

        public void Dispose()
        {
            
        }

        public void MessageHandle(string topic, string message)
        {
            
        }

        public void Publish(IRemoteEventData eventData)
        {
            
        }

        public void Publish(string topic, IRemoteEventData eventData)
        {
            
        }

        public Task PublishAsync(IRemoteEventData eventData)
        {
            return Task.FromResult(0);
        }

        public Task PublishAsync(string topic, IRemoteEventData eventData)
        {
            return Task.FromResult(0);
        }

        public void Subscribe(string topic)
        {
            
        }

        public void Subscribe(IEnumerable<string> topics)
        {
            
        }

        public Task SubscribeAsync(string topic)
        {
            return Task.FromResult(0);
        }

        public Task SubscribeAsync(IEnumerable<string> topics)
        {
            return Task.FromResult(0);
        }

        public void Unsubscribe(string topic)
        {
            
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            
        }

        public void UnsubscribeAll()
        {
            
        }

        public Task UnsubscribeAllAsync()
        {
            return Task.FromResult(0);
        }

        public Task UnsubscribeAsync(string topic)
        {
            return Task.FromResult(0);
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return Task.FromResult(0);
        }
    }
}
