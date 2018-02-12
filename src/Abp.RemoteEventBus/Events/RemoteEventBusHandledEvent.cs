using Abp.Events.Bus;

namespace Abp.RemoteEventBus.Events
{
    public class RemoteEventBusHandledEvent : RemoteEventBusHandleEvent
    {
        public RemoteEventBusHandledEvent(RemoteEventArgs eventArgs)
            : base(eventArgs)
        {

        }
    }
}
