using Abp.Modules;
using Abp.Reflection.Extensions;
using Commons.Pool;

namespace Abp.RemoteEventBus.Redis
{
    [DependsOn(typeof(AbpRemoteEventBusModule))]
    public class AbpRemoteEventBusRabbitMQModule: AbpModule
    {
        public override void Initialize()
        {
            IocManager.Register<IPoolManager, PoolManager>();
            IocManager.RegisterAssemblyByConvention(typeof(AbpRemoteEventBusRabbitMQModule).GetAssembly());
        }
    }
}