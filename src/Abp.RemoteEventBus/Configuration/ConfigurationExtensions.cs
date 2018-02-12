using Abp.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus
{
    public static class ConfigurationExtensions
    {
        public static IRemoteEventBusConfiguration RemoteEventBus(this IModuleConfigurations configuration)
        {
            return configuration.AbpConfiguration.GetOrCreate("Modules.Abp.RemoteEventBus", () => configuration.AbpConfiguration.IocManager.Resolve<IRemoteEventBusConfiguration>());
        }
    }
}
