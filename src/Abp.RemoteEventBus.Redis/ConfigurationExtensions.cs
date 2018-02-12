using System;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Abp.RemoteEventBus.Redis
{
    public  static class ConfigurationExtensions
    {
        public static IRedisConfiguration UseRedis(this IRemoteEventBusConfiguration configuration)
        {
            var iocManager = configuration.AbpStartupConfiguration.IocManager;
            
            iocManager.IocContainer.Register(
                Component.For<IRemoteEventPublisher>()
                    .ImplementedBy<RedisRemoteEventPublisher>()
                    .LifestyleSingleton()
                    .IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IRemoteEventSubscriber>()
                    .ImplementedBy<RedisRemoteEventSubscriber>()
                    .LifestyleSingleton()
                    .IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IRemoteEventBus>()
                    .ImplementedBy<RemoteEventBus>()
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );

            iocManager.RegisterIfNot<IRedisConfiguration, RedisConfiguration>();

            return iocManager.Resolve<IRedisConfiguration>();
        }
    }
}