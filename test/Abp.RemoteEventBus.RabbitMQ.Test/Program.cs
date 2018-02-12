using System;
using System.Threading.Tasks;
using Abp.Castle.Logging.Log4Net;
using Castle.Facilities.Logging;

namespace Abp.RemoteEventBus.RabbitMQ.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = AbpBootstrapper.Create<RabbitMQTestModule>();

            bootstrapper.IocManager.IocContainer.AddFacility<LoggingFacility>(f =>
                f.UseAbpLog4Net().WithConfig("log4net.config"));

            bootstrapper.Initialize();
            
            var remoteEventBus=bootstrapper.IocManager.Resolve<IRemoteEventBus>();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {       
                    const string type = "Type_Test";
                    const string topic = "Topic_Test";
                    var eventDate = new RemoteEventData(type)
                    {
                        Data = {["playload"] = DateTime.Now}
                    };
                    remoteEventBus.Publish(topic, eventDate);

                    Task.Delay(1000).Wait();
                }
            });

            Console.WriteLine("Any key exit");
            Console.ReadKey();
        }
    }
}