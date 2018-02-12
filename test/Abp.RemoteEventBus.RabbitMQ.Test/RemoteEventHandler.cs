using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.RemoteEventBus.RabbitMQ.Test
{
    [RemoteEventHandler(ForType = "Type_Test", ForTopic = "Topic_Test")]
    public class RemoteEventHandler : IRemoteEventHandler, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public RemoteEventHandler()
        {
            Logger = NullLogger.Instance;
        }

        public void HandleEvent(RemoteEventArgs eventArgs)
        {
            Logger.Info("receive " + eventArgs.EventData.Data["playload"]);
        }
    }
}