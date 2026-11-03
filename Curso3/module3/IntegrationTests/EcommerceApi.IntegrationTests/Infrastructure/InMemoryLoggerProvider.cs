// EcommerceApi.IntegrationTests/Infrastructure/InMemoryLoggerProvider.cs
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace EcommerceApi.IntegrationTests.Infrastructure;

public sealed record CapturedLog(
    string Category,
    LogLevel Level,
    EventId EventId,
    Exception? Exception,
    string Message);

public sealed class InMemoryLoggerProvider : ILoggerProvider
{
    public ConcurrentQueue<CapturedLog> Logs { get; } = new();

    public ILogger CreateLogger(string categoryName) => new CapturingLogger(this, categoryName);

    public void Dispose() { }

    private sealed class CapturingLogger : ILogger
    {
        private readonly InMemoryLoggerProvider _owner;
        private readonly string _category;

        public CapturingLogger(InMemoryLoggerProvider owner, string category)
        {
            _owner = owner;
            _category = category;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _owner.Logs.Enqueue(new CapturedLog(
                _category, logLevel, eventId, exception, formatter(state, exception)));
        }
    }
}