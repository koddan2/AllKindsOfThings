namespace Debus;
public interface ICommand { }
public interface ICommandBroker
{
    Task SendAsync<T>(T command)
        where T : ICommand;
}
public interface IMessageBus
{
    Task PublishAsync(string topic, object message, IDictionary<string, object> attributes);
    Task SubscribeAsync(string topic, Func<object, IDictionary<string, object>, Task> messageHandler);
}
public record EncodedMessage(Type Type, byte[] Data);
public interface IEncryptedMessage
{
    byte[] Ciphertext { get; }
}
public interface IDebusEncryption
{
    byte[] Decrypt(IEncryptedMessage encryptedMessage);
    IEncryptedMessage Encrypt(byte[] data);
}
public interface IDebusCodec
{
    EncodedMessage Encode(Type type, object data);
    object Decode(EncodedMessage encodedMessage);

    EncodedMessage Encode<T>(T data);
    T Decode<T>(EncodedMessage encodedMessage);
}

public record DebusMessageReceivedArgs(EncodedMessage EncodedMessage);
internal record DebusControlMessageReceivedArgs(EncodedMessage EncodedMessage);
public class Debus
{
    public event EventHandler<DebusMessageReceivedArgs>? MessageReceived;
    internal event EventHandler<DebusControlMessageReceivedArgs>? ControlMessageReceived;

    public IDebusCodec Codec { get; }
    internal IDebusCodec InternalCodec { get; }
}