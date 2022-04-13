using Serilog;
using System.Runtime.CompilerServices;

namespace Akot.Jelly.Lib
{
    public static class LoggerExtensions
    {
        public static ILogger Here<T>(
            this ILogger logger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return logger.ForContext(new CallSiteEnricher(memberName, sourceFilePath, sourceLineNumber));
        }
    }
}