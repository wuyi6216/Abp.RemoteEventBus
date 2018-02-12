namespace Abp.RemoteEventBus
{
    public interface IRemoteEventSerializer
    {
        T Deserialize<T>(string value);

        string Serialize(object value);
    }
}