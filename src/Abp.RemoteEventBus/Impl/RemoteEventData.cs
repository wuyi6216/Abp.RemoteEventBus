using Abp.RemoteEventBus;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Events.Bus;

namespace Abp.RemoteEventBus
{
    [Serializable]
    public sealed class RemoteEventData : EventData, IRemoteEventData
    {
        public Dictionary<string, object> Data { get; set; }

        public string Type { get; set; }

        private RemoteEventData()
        {
            Data = new Dictionary<string, object>();
        }

        public RemoteEventData(string type) : this()
        {
            Type = type;
        }

        public RemoteEventData(string type, Dictionary<string, object> data) : this()
        {
            Type = type;
            Data = data;
        }
    }
}
