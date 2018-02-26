using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.RemoteEventBus.Kafka
{
    [DependsOn(typeof(AbpRemoteEventBusModule))]
    public class AbpRemoteEventBusKafkaModule: AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpRemoteEventBusKafkaModule).GetAssembly());
        }
    }
}