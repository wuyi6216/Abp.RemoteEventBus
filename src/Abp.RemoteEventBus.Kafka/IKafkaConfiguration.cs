using System;

namespace Abp.RemoteEventBus.Kafka
{
    public interface IKafkaConfiguration
    {
        IKafkaConfiguration Configure(Action<IKafkaSetting> configureAction);

        IKafkaConfiguration Configure(IKafkaSetting setting);
    }
}
