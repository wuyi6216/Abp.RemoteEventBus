using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus.Redis
{
    public interface IRedisSetting
    {
        string Server { get; set; }

        int DatabaseId { get; set; }
    }
}
