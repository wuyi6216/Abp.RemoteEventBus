## How it work?

### RemoteEventBus
Define the key interface

```
    public class RemoteEventBus : IRemoteEventBus
    {
        public ILogger Logger { get; set; }

        private readonly IEventBus _eventBus;
        private readonly IRemoteEventPublisher _publisher;
        private readonly IRemoteEventSubscriber _subscriber;
        private readonly IRemoteEventTopicSelector _topicSelector;
        private readonly IRemoteEventSerializer _remoteEventSerializer;
```
### ConfigurationExtensions
Configuration specific implementation,such as kafka
```
    public static class ConfigurationExtensions
    {
        public static IKafkaConfiguration UseKafka(this IRemoteEventBusConfiguration configuration)
        {
            var iocManager = configuration.AbpStartupConfiguration.IocManager;

            iocManager.IocContainer.Register(
                Component.For<IRemoteEventPublisher>().ImplementedBy<KafkaRemoteEventPublisher>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IRemoteEventSubscriber>().ImplementedBy<KafkaRemoteEventSubscriber>()
                    .LifestyleSingleton().IsDefault()
            );
            iocManager.IocContainer.Register(
                Component.For<IRemoteEventBus>()
                    .ImplementedBy<RemoteEventBus>()
                    .Named(Guid.NewGuid().ToString())
                    .LifestyleSingleton()
                    .IsDefault()
            );
```
### MessageHandle

```
        public virtual void MessageHandle(string topic, string message)
        {
            Logger.Debug($"Receive message on topic {topic}");
            try
            {
                var eventData = _remoteEventSerializer.Deserialize<RemoteEventData>(message);
                var eventArgs = new RemoteEventArgs(eventData, topic, message);
                _eventBus.Trigger(this, new RemoteEventBusHandlingEvent(eventArgs));
                _eventBus.Trigger(this, eventArgs);
                _eventBus.Trigger(this, new RemoteEventBusHandledEvent(eventArgs));
            }
```
When a message is received from the message middleware, the event RemoteEventArgs is triggered on Abp local event bus.then **AttributeRemoteEventHandler** will handle this event,you can define other handler by implementing **IEventHandler<RemoteEventArgs>**.

### AttributeRemoteEventHandler
```
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
```


