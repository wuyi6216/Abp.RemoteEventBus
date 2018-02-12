using System;
using Abp.Events.Bus.Exceptions;

namespace Abp.RemoteEventBus.Exceptions
{
    [Serializable]
    public class RemoteEventMessageHandleExceptionData : AbpHandledExceptionData
    {
        public string Topic { get; set; }

        public string Message { get; set; }

        public RemoteEventMessageHandleExceptionData(Exception exception, string topic, string message) : base(exception)
        {
            Topic = topic;
            Message = message;
        }
    }
}
