using System;

namespace Abp.RemoteEventBus.Redis
{
    public interface IRedisConfiguration
    {
        IRedisConfiguration Configure(Action<IRedisSetting> configureAction);

        IRedisConfiguration Configure(IRedisSetting setting);
    }
}
