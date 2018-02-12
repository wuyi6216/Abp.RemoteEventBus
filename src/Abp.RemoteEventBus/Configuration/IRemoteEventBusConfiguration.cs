using Abp.Configuration.Startup;

namespace Abp.RemoteEventBus
{
    public interface IRemoteEventBusConfiguration
    {
        IAbpStartupConfiguration AbpStartupConfiguration { get; }

        IRemoteEventBusConfiguration AutoSubscribe();
    }
}