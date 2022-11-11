using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZT.Cli
{
    internal class Globals
    {
        internal static readonly ILoggerFactory DztLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        internal static ILogger logger = DztLoggerFactory.CreateLogger<Program>();
    }
}
