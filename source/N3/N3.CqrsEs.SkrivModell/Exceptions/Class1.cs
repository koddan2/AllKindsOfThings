namespace N3.CqrsEs.SkrivModell.Exceptions
{
    [Serializable]
    public class GenerellKonfliktException : Exception
    {
        public GenerellKonfliktException() { }

        public GenerellKonfliktException(string message)
            : base(message) { }

        public GenerellKonfliktException(string message, Exception inner)
            : base(message, inner) { }

        protected GenerellKonfliktException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context
        )
            : base(info, context) { }

        public string? Korrelation { get; set; }
    }
}
