using Microsoft.Extensions.Logging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DZT.Gui
{
    public sealed class WpfLogger : ILogger
    {
        private readonly string _name;
        private readonly TextBox? _infoTextBox;

        public WpfLogger(string name, TextBox? infoTextBox)
        {
            _name = name;
            _infoTextBox = infoTextBox;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            if (_infoTextBox is null)
            {
                return;
            }

            _infoTextBox.Text += ($"[{eventId.Id, 2}: {logLevel, -12}]");
            _infoTextBox.Text += ($"     {_name} - ");
            _infoTextBox.Text += ($"{formatter(state, exception)}");
            _infoTextBox.Text += ("\r\n");
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(delegate { })
            );
        }
    }
}
