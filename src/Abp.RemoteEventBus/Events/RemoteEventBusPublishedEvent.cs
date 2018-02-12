using Abp.Events.Bus;

namespace Abp.RemoteEventBus.Events
{
    public class RemoteEventBusPublishedEvent : RemoteEventBusPublishEvent
    {
        public RemoteEventBusPublishedEvent(IRemoteEventData eventData)
            : base(eventData)
        {

        }
    }
}
