using Serilog.Core;
using Serilog.Events;

namespace Akot.Jelly.Lib
{
    record CallSiteEnricher(string MemberName, string SourceFilePath, int SourceLineNumber): ILogEventEnricher
    {
        void ILogEventEnricher.Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(MemberName), MemberName));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(SourceFilePath), SourceFilePath));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(SourceLineNumber), SourceLineNumber));
        }
    }
}