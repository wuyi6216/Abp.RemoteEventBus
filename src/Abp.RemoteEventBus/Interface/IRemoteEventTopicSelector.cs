using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus
{
    public interface IRemoteEventTopicSelector
    {
        string SelectTopic(IRemoteEventData eventData);

        void SetMapping<T>(string topic) where T : IRemoteEventData;
    }
}
