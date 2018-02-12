# Abp.RemoteEventBus
## What’s this?

受到[Abp](https://github.com/FJQBT/ABP)事件总线的灵感而开发的一个分布式的事件总线，可以跨应用触发事件。基于发布/订阅模式，消息的传递可以通过redis，rabbitmq，kafka等实现。你还能非常容易的实现你自己的方式通过实现指定接口。支持控制台和web应用。

A distributed event bus, inspired by the [Abp](https://github.com/FJQBT/ABP) event bus, can trigger events across applications. Based on publish / subscribe mode, the message can be passed through redis, rabbitmq, kafka and so on. You can also very easily implement your own way by implementing the specified interface. Support in console and web applications.

## How to use?

### Publish

```
    var eventDate = new RemoteEventData("Type_Test")
        {
            Data ={["playload"]=DateTime.Now}
        };
    remoteEventBus.Publish("Topic_Test", eventDate);

```

### Subscribe

```
    [RemoteEventHandler(ForType = "Type_Test", ForTopic = "Topic_Test")]
    public class RemoteEventHandler : IRemoteEventHandler, ITransientDependency
    {
        public void HandleEvent(RemoteEventArgs eventArgs)
        {
            Logger.Info("receive " + eventArgs.EventData.Data["playload"]);
        }
    }
```
### Configuration

```
    [DependsOn(typeof(AbpRemoteEventBusKafkaModule))]
    //[DependsOn(typeof(AbpRemoteEventBusRabbitMQModule))]
    //[DependsOn(typeof(AbpRemoteEventBusRedisModule))]
    public class DemoModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DemoModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            // use kafka
            Configuration.Modules.RemoteEventBus().UseKafka().Configure((setting) =>
            {
                setting.Properties.Add("bootstrap.servers", "192.168.188.142:9092");
                setting.Properties.Add("group.id", "App-Test");
            });
            
            // use rabbitmq
            //Configuration.Modules.RemoteEventBus().UseRabbitMQ().Configure(setting =>
            //{
                //setting.Url = "amqp://guest:guest@127.0.0.1:5672/";
            //});
            
            // use redis
            //Configuration.Modules.RemoteEventBus().UseRedis().Configure((setting) =>
            //{
            //    setting.Server = "127.0.0.1:6379";
            //});
            
            // enable auto subscribe
            // will scan class which use RemoteEventHandlerAttribute and auto subscribe topic base the attribute info
            Configuration.Modules.RemoteEventBus().AutoSubscribe();
        }
    }
```
### Demo
see Abp.RemoteEventBus.RabbitMQ.Test