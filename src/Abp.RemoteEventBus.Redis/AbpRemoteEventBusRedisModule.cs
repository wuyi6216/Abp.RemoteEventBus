using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.RemoteEventBus.Redis
{
    [DependsOn(typeof(AbpRemoteEventBusModule))]
    public class AbpRemoteEventBusRedisModule: AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpRemoteEventBusRedisModule).GetAssembly());
        }
    }
}