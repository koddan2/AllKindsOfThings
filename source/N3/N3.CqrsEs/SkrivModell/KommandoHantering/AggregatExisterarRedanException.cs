namespace N3.CqrsEs.SkrivModell.Hantering
{
    [Serializable]
    public class AggregatExisterarRedanException : Exception
    {
        public AggregatExisterarRedanException() { }
        public AggregatExisterarRedanException(string message) : base(message) { }
        public AggregatExisterarRedanException(string message, Exception inner) : base(message, inner) { }
        protected AggregatExisterarRedanException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public Guid Id { get; init; }
    }
}
