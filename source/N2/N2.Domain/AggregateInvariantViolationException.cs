namespace N2.Domain
{
	[Serializable]
	public class AggregateInvariantViolationException : Exception
	{
		public AggregateInvariantViolationException() { }
		public AggregateInvariantViolationException(string message) : base(message) { }
		public AggregateInvariantViolationException(string message, Exception inner) : base(message, inner) { }
		protected AggregateInvariantViolationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		public IReadOnlyCollection<AggregateInvariantViolated> ViolatedInvariants { get; set; }
	}
}