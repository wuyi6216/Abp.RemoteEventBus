using Abp.Events.Bus;

namespace Abp.RemoteEventBus.Events
{
    public abstract class RemoteEventBusHandleEvent : RemoteEventBusEvent
    {
        public RemoteEventArgs RemoteEventArgs { get; private set; }

        public RemoteEventBusHandleEvent(RemoteEventArgs eventArgs)
        {
            RemoteEventArgs = eventArgs;
        }
    }
}
