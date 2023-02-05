namespace N2.EventSourcing.Common
{
	[Serializable]
	public class ExpectationFailedException : Exception
	{
		public ExpectationFailedException() { }
		public ExpectationFailedException(string message) : base(message) { }
		public ExpectationFailedException(string message, Exception inner) : base(message, inner) { }
		protected ExpectationFailedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		public Code? ErrorCode { get; init; }
		public enum Code
		{
			StreamExists = 1,
			StreamDoesNotExist = 2,
		}
	}
}