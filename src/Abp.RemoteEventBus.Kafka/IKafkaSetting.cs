using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus.Kafka
{
    public interface IKafkaSetting
    {
        Dictionary<string, object> Properties { get; set; }
    }
}
