using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.RemoteEventBus.Redis;

namespace Abp.RemoteEventBus.RabbitMQ.Test
{
    [DependsOn(typeof(AbpRemoteEventBusRabbitMQModule))]
    public class RabbitMQTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RabbitMQTestModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            Configuration.Modules.RemoteEventBus().UseRabbitMQ().Configure(setting =>
            {
                setting.Url = "amqp://guest:guest@127.0.0.1:5672/";
            });

            Configuration.Modules.RemoteEventBus().AutoSubscribe();
        }
    }
}