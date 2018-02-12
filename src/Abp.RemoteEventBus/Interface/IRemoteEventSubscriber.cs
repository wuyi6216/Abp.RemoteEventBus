using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus
{
    public interface IRemoteEventSubscriber : IDisposable
    {
        void Subscribe(IEnumerable<string> topics, Action<string, string> handler);

        Task SubscribeAsync(IEnumerable<string> topics, Action<string, string> handler);

        void Unsubscribe(IEnumerable<string> topics);

        Task UnsubscribeAsync(IEnumerable<string> topics);

        void UnsubscribeAll();

        Task UnsubscribeAllAsync();
    }
}
