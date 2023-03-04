namespace N3.CqrsEs
{
    public static class Channels
    {
        public const string N3DomainInternal = "n3-domain-internal";
        public const string N3DomainCommon = "n3-domain-common";
    }
    public static class Topics
    {
        public const string PingPong = "ping-pong";
        public const string ÄrendeImportJobb = "jobb-ärende-import";
        public const string ÄrendeImport = "ärende-import";
    }
}
