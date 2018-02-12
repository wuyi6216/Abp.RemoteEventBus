namespace Abp.RemoteEventBus
{
    public interface IRemoteEventHandler
    {
        void HandleEvent(RemoteEventArgs eventArgs);
    }
}
