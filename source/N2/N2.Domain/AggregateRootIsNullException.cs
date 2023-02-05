namespace N2.Domain;

[Serializable]
public class AggregateRootIsNullException : Exception
{
	public AggregateRootIsNullException() { }
	public AggregateRootIsNullException(string message) : base(message) { }
	public AggregateRootIsNullException(string message, Exception inner) : base(message, inner) { }
	protected AggregateRootIsNullException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}