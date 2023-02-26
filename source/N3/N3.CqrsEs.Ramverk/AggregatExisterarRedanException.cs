using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.Ramverk
{
    [Serializable]
    [InitRequired]
    public class AggregatExisterarRedanException : Exception
    {
        public AggregatExisterarRedanException() { }

        public AggregatExisterarRedanException(string message)
            : base(message) { }

        public AggregatExisterarRedanException(string message, Exception inner)
            : base(message, inner) { }

        protected AggregatExisterarRedanException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context
        )
            : base(info, context) { }

        public string Aggregat { get; init; }
        public UnikIdentifierare AggregatIdentifierare { get; init; }
    }

    [Serializable]
    [InitRequired]
    public class AggregatHarInteSkapatsException : Exception
    {
        public AggregatHarInteSkapatsException() { }

        public AggregatHarInteSkapatsException(string message)
            : base(message) { }

        public AggregatHarInteSkapatsException(string message, Exception inner)
            : base(message, inner) { }

        protected AggregatHarInteSkapatsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context
        )
            : base(info, context) { }

        public string Aggregat { get; init; }
        public UnikIdentifierare? AggregatIdentifierare { get; init; }
    }
}
