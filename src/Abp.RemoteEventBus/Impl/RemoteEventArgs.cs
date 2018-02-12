using System;
using System.Collections.Generic;
using System.Text;
using Abp.Events.Bus;

namespace Abp.RemoteEventBus
{
    public class RemoteEventArgs : EventArgs, IEventData
    {
        public IRemoteEventData EventData { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }

        public bool Suspended { get; set; }

        public DateTime EventTime { get; set; }

        public object EventSource { get; set; }

        public RemoteEventArgs(IRemoteEventData eventData, string topic,string message)
        {
            EventData = eventData;
            Message = message;
            Topic = topic;
            EventTime = DateTime.Now;
        }
    }
}
