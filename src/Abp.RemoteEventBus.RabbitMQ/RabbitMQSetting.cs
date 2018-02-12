using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;

namespace Abp.RemoteEventBus.RabbitMQ
{
    public class RabbitMQSetting : IRabbitMQSetting, ITransientDependency
    {
        public string Url { get; set; }

        public bool AutomaticRecoveryEnabled { get; set; }

        public int InitialSize { get; set; }

        public int MaxSize { get; set; }

        public RabbitMQSetting()
        {
            AutomaticRecoveryEnabled = true;
            InitialSize = 0;
            MaxSize = 10;
        }
    }
}
