using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus.Redis
{
    public class RedisSetting : IRedisSetting, ITransientDependency
    {
        public string Server { get; set; }

        public int DatabaseId { get; set; }
    }
}
