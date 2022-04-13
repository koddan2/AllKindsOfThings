using SmartAnalyzers.CSharpExtensions.Annotations;

#pragma warning disable IDE1006 // Naming Styles
[InitRequired]
internal class application_event
{
    public string id { get; set; }
    public string stream_id { get; set; }
    public uint sequence_number { get; set; }
    public string event_type { get; set; }
    public string payload { get; set; }
    public string created_at { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles