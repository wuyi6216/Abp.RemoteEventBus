using Abp.Dependency;
using Abp.RemoteEventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.RemoteEventBus
{
    public class RemoteEventTopicSelector : IRemoteEventTopicSelector, ISingletonDependency
    {
        public const string TOPIC_DEFAULT = "TOPIC_DEFAULT";

        private readonly Dictionary<Type, string> _mapping;

        public RemoteEventTopicSelector()
        {
            _mapping = new Dictionary<Type, string>();
        }

        public void SetMapping<T>(string topic) where T : IRemoteEventData
        {
            _mapping[typeof(T)] = topic;
        }

        public string SelectTopic(IRemoteEventData eventData)
        {
            if (eventData is IHasTopicRemoteEventData)
            {
                return (eventData as IHasTopicRemoteEventData).Topic;
            }
            foreach (var item in _mapping)
            {
                if (item.Key.IsAssignableFrom(eventData.GetType()))
                {
                    return item.Value;
                }
            }
            return TOPIC_DEFAULT;
        }
    }
}
