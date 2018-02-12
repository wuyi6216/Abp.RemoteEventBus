using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RemoteEventBus
{
    public interface IRemoteEventPublisher: IDisposable
    {
        void Publish(string topic, IRemoteEventData remoteEventData);

        Task PublishAsync(string topic, IRemoteEventData remoteEventData);
    }
}
