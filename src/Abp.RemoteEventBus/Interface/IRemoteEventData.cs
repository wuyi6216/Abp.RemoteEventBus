using System.Collections.Generic;
using Abp.Events.Bus;

namespace Abp.RemoteEventBus
{
    public interface IRemoteEventData : IEventData
    {
        string Type { get; set; }

        Dictionary<string, object> Data { get; set; }
    }
}
