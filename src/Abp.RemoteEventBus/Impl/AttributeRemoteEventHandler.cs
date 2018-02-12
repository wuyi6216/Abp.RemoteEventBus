using Abp.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;
using Abp.RemoteEventBus.Exceptions;

namespace Abp.RemoteEventBus.Impl
{
    public class AttributeRemoteEventHandler : IEventHandler<RemoteEventArgs>, ISingletonDependency
    {
        public ILogger Logger { get; set; }

        private readonly TypeFinder _typeFinder;

        private readonly Dictionary<string, List<Tuple<Type, RemoteEventHandlerAttribute>>> _typeMapping;

        private readonly IIocResolver _iocResolver;

        private readonly IEventBus _eventBus;

        public AttributeRemoteEventHandler(TypeFinder typeFinder, IIocResolver iocResolver, IEventBus eventBus)
        {
            Logger = NullLogger.Instance;

            _typeFinder = typeFinder;
            _iocResolver = iocResolver;
            _eventBus = eventBus;

            _typeMapping = new Dictionary<string, List<Tuple<Type, RemoteEventHandlerAttribute>>>();

            _typeFinder.Find(type => Attribute.IsDefined(type, typeof(RemoteEventHandlerAttribute), false) && typeof(IRemoteEventHandler).IsAssignableFrom(type))
                .ToList().ForEach(type =>
                {
                    var attribute = Attribute.GetCustomAttribute(type, typeof(RemoteEventHandlerAttribute)) as RemoteEventHandlerAttribute;
                    var key = attribute.ForType;
                    var item = new Tuple<Type, RemoteEventHandlerAttribute>(type, attribute);
                    if (_typeMapping.ContainsKey(key))
                    {
                        var list = _typeMapping[key];
                        list.Add(item);
                    }
                    else
                    {
                        _typeMapping.Add(key, new List<Tuple<Type, RemoteEventHandlerAttribute>>(new[] { item }));
                    }
                });
        }

        public void HandleEvent(RemoteEventArgs eventArgs)
        {
            var key = eventArgs.EventData.Type;
            if (_typeMapping.ContainsKey(key))
            {
                var tuples = _typeMapping[key].OrderBy(p => p.Item2.Order).ToList();
                foreach (var tuple in tuples)
                {
                    if (tuple.Item2.OnlyHandleThisTopic && eventArgs.Topic != tuple.Item2.ForTopic)
                    {
                        continue;
                    }

                    try
                    {
                        var handler = (IRemoteEventHandler)_iocResolver.Resolve(tuple.Item1);
                        handler.HandleEvent(eventArgs);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Exception occurred when handle remoteEventArgs", ex);

                        _eventBus.Trigger(this, new RemoteEventHandleExceptionData(ex, eventArgs));

                        if (tuple.Item2.SuspendWhenException)
                        {
                            eventArgs.Suspended = true;
                        }
                    }

                    if (eventArgs.Suspended)
                    {
                        break;
                    }
                }
            }
        }
    }
}
