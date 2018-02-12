using System;

namespace Abp.RemoteEventBus
{
    public class NullRemoteEventHandler : IRemoteEventHandler
    {
        public static NullRemoteEventHandler Instance { get { return SingletonInstance; } }
        private static readonly NullRemoteEventHandler SingletonInstance = new NullRemoteEventHandler();

        public void HandleEvent(RemoteEventArgs eventData)
        {
            
        }
    }
}
