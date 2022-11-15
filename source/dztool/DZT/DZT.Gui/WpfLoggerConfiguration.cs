using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DZT.Gui
{
    public sealed class WpfLoggerConfiguration
    {
        public int EventId { get; set; }

        public Dictionary<LogLevel, ConsoleColor> LogLevelToColorMap { get; set; } = new()
        {
            [LogLevel.Information] = ConsoleColor.Green
        };
    }
}
