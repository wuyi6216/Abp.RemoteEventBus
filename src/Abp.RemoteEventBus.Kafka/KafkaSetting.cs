using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus.Kafka
{
    public class KafkaSetting : IKafkaSetting, ITransientDependency
    {
        public Dictionary<string, object> Properties { get; set; }

        public KafkaSetting()
        {
            Properties = new Dictionary<string, object>();
            Properties.Add("enable.auto.commit", "true");
            Properties.Add("auto.commit.interval.ms", "1000");
            Properties.Add("session.timeout.ms", "30000");
        }
    }
}
