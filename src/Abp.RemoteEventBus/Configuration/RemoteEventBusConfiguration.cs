using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Core.Logging;

namespace Abp.RemoteEventBus.RabbitMQ
{
    public class RemoteEventBusConfiguration : IRemoteEventBusConfiguration, ISingletonDependency
    {
        private readonly IAbpStartupConfiguration _configuration;
        private readonly TypeFinder _typeFinder;

        public ILogger Logger { get; set; }

        public IAbpStartupConfiguration AbpStartupConfiguration
        {
            get { return _configuration; }
        }

        public RemoteEventBusConfiguration(IAbpStartupConfiguration configuration, TypeFinder typeFinder)
        {
            _configuration = configuration;
            _typeFinder = typeFinder;

            Logger = NullLogger.Instance;
        }

        public IRemoteEventBusConfiguration AutoSubscribe()
        {
            var topics = new List<string>();
            _typeFinder.Find(type => Attribute.IsDefined(type, typeof(RemoteEventHandlerAttribute), false) && typeof(IRemoteEventHandler).IsAssignableFrom(type))
            .ToList().ForEach(type =>
            {
                var attribute = Attribute.GetCustomAttribute(type, typeof(RemoteEventHandlerAttribute)) as RemoteEventHandlerAttribute;
                if (!string.IsNullOrWhiteSpace(attribute.ForTopic) && !topics.Contains(attribute.ForTopic))
                {
                    topics.Add(attribute.ForTopic);
                }
            });

            Logger.Info($"auto subscribe topics {string.Join(",", topics)}");

            var remoteEventBus = _configuration.IocManager.Resolve<IRemoteEventBus>();
            remoteEventBus.Subscribe(topics);

            return this;
        }
    }
}
