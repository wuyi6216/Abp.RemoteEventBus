using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus
{
    public interface IHasTopicRemoteEventData
    {
        string Topic { get; set; }
    }
}
