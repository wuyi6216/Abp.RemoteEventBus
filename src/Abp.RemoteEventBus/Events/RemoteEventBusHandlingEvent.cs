namespace Abp.RemoteEventBus.Events
{
    public class RemoteEventBusHandlingEvent : RemoteEventBusHandleEvent
    {
        public RemoteEventBusHandlingEvent(RemoteEventArgs eventArgs)
            : base(eventArgs)
        {

        }
    }
}
