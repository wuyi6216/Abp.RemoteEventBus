using System;
using System.Collections.Generic;
using System.Text;
using Abp.Events.Bus.Exceptions;

namespace Abp.RemoteEventBus.Exceptions
{
    [Serializable]
    public class RemoteEventHandleExceptionData : AbpHandledExceptionData
    {
        public RemoteEventArgs EventArgs { get; set; }

        public RemoteEventHandleExceptionData(Exception exception, RemoteEventArgs eventArgs) : base(exception)
        {
            EventArgs = eventArgs;
        }
    }
}
