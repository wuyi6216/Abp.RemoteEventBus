namespace Abp.RemoteEventBus.Events
{
    public class RemoteEventBusPublishingEvent: RemoteEventBusPublishEvent
    {
        public RemoteEventBusPublishingEvent(IRemoteEventData eventData)
            : base(eventData)
        {

        }
    }
}
