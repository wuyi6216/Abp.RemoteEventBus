using System;
using Abp.Dependency;
using Abp.RemoteEventBus.Kafka;
using Castle.MicroKernel.Registration;

namespace Abp.RemoteEventBus.Kafka
{
    public static class ConfigurationExtensions
    {
        public static IKafkaConfiguration UseKafka(this IRemoteEventBusConfiguration configuration)
        {
            var iocManager = configuration.AbpStartupConfiguration.IocManager;

            iocManager.IocContainer.Register(
                Component.For<IRemoteEventPublisher>().ImplementedBy<KafkaRemoteEventPublisher>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IRemoteEventSubscriber>().ImplementedBy<KafkaRemoteEventSubscriber>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IRemoteEventBus>()
                    .ImplementedBy<RemoteEventBus>()
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            iocManager.RegisterIfNot<IKafkaConfiguration, KafkaConfiguration>();

            return iocManager.Resolve<IKafkaConfiguration>();
        }
    }
}