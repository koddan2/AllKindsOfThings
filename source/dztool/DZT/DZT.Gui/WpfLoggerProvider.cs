using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Windows.Controls;

namespace DZT.Gui
{
    [ProviderAlias("WpfOutput")]
    public sealed class WpfLoggerProvider : ILoggerProvider
    {
        private TextBox? _infoTextBox;
        private WpfLoggerConfiguration? _currentConfig;
        private readonly IDisposable? _onChangeToken;
        private readonly ConcurrentDictionary<string, WpfLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        public WpfLoggerProvider(IOptionsMonitor<WpfLoggerConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public WpfLoggerProvider(TextBox infoTextBox)
        {
            _infoTextBox = infoTextBox;
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new WpfLogger(name, _infoTextBox));

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
