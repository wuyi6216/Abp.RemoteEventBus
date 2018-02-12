using Abp.Events.Bus;

namespace Abp.RemoteEventBus.Events
{
    public abstract class RemoteEventBusPublishEvent : RemoteEventBusEvent
    {
        public IRemoteEventData RemoteEventData { get; private set; }

        public RemoteEventBusPublishEvent(IRemoteEventData eventData)
        {
            RemoteEventData = eventData;
        }
    }
}
